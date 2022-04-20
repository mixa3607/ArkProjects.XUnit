using System;
using System.Linq;
using ArkProjects.XUnit.Json;
using FluentAssertions;
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

        public record TestModelRecord
        {
            [XUnitJsonFileName]
            public string File { get; set; }

            [XUnitJsonCaseIndex]
            public int? Idx { get; set; }

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
            model.File.Should().Be("./files/tests/JsonDataAttributeTests/MultiParam2.json");
            if (model.Idx == 0)
            {
                value.Should().Be("test value 1");
                value2.Should().Be(1);
                model.Value1.Should().Be("1v1");
                model.Value2.Should().Be("1v2");
            }
            else if (model.Idx == 1)
            {
                value.Should().Be("test value 2");
                value2.Should().Be(2);
                model.Value1.Should().Be("2v1");
                model.Value2.Should().Be("2v2");
            }
            else
            {
                throw new Exception();
            }
        }

        [Theory]
        [JsonData("./files/tests/{class}/SingleParam2.json")]
        public void SingleParam2(TestModelRecord model)
        {
            model.File.Should().Be("./files/tests/JsonDataAttributeTests/SingleParam2.json");
            if (model.Idx == 0)
            {
                model.Value1.Should().Be("1v1");
                model.Value2.Should().Be("1v2");
            }
            else if (model.Idx == 1)
            {
                model.Value1.Should().Be("2v1");
                model.Value2.Should().Be("2v2");
            }
            else
            {
                throw new Exception();
            }
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