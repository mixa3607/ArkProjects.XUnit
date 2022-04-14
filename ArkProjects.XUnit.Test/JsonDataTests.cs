using System;
using System.Linq;
using ArkProjects.XUnit.Json;
using Xunit;

namespace ArkProjects.XUnit.Test
{
    public class JsonDataAttributeTests
    {
        public class TestModel : XUnitJsonSerializable
        {
            [XUnitJsonFileName]
            public string File { get; set; }

            [XUnitJsonCaseIndex]
            public int Idx { get; set; }

            public string Value1 { get; set; }
            public string Value2 { get; set; }

            public override string ToString()
            {
                return $"[{Idx}]{File}";
            }
        };

        private void SingleParam(string value)
        {
        }

        [Theory]
        [JsonData("./files/tests/{class}/MultiParam2.json")]
        public void MultiParam2(string value, int value2, TestModel model)
        {
        }

        [Theory]
        [JsonData("./files/tests/{class}/MultiParam.json")]
        public void MultiParam(string value, TestModel model)
        {
        }

        [Fact]
        public void SingleParamTest()
        {
            var attr = new JsonDataAttribute("./files/tests/{class}/SingleParam.json");
            var methodInfo = ((Action<string>)SingleParam).Method;
            var testCases = attr.GetData(methodInfo).ToArray();

            Assert.Equal(2, testCases.Length);
            Assert.True(testCases.All(x => x.Length == 1));
            Assert.Equal("test value 1", (string)testCases[0][0]);
            Assert.Equal("test value 2", (string)testCases[1][0]);
        }


        [Fact]
        public void MultiParamTest()
        {
            var attr = new JsonDataAttribute("./files/tests/{class}/MultiParam.json");
            var methodInfo = ((Action<string, TestModel>)MultiParam).Method;
            var testCases = attr.GetData(methodInfo).ToArray();

            Assert.Equal(2, testCases.Length);
            Assert.True(testCases.All(x => x.Length == 2));

            Assert.Equal("test value 1", (string)testCases[0][0]);
            Assert.Equal("1v1", ((TestModel)testCases[0][1])!.Value1);
            Assert.Equal("1v2", ((TestModel)testCases[0][1])!.Value2);

            Assert.Equal("test value 2", (string)testCases[1][0]);
            Assert.Equal("2v1", ((TestModel)testCases[1][1])!.Value1);
            Assert.Equal("2v2", ((TestModel)testCases[1][1])!.Value2);
        }
    }
}