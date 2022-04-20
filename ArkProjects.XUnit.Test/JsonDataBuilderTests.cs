using ArkProjects.XUnit.Json;
using ArkProjects.XUnit.Json.JsonBuilder;
using Newtonsoft.Json;
using Xunit;

namespace ArkProjects.XUnit.Test
{
    public class JsonDataBuilderTests
    {
        [Fact]
        public void Test1()
        {
            var builder = new JsonDataBuilder(JsonTestDataType.Auto);
            builder.ForMethod<JsonDataBuilderTests>(x => x.TargetMethod2(null, 0));
            builder.AddCase(x => x
                .SetNamed("testData", "abc")
                .SetNamed("testInt", 123)
            );
            builder.Validate();
            var t = builder.Serialize();
        }

        [Fact]
        public void Test2()
        {
            var builder = new JsonDataBuilder(JsonTestDataType.Auto);
            builder
                .ForMethod<JsonDataBuilderTests>(x => x.TargetMethod1(null))
                .AddCase(x => x.AppendPositioned("aaaa"))
                .AddCase(x => x.AppendPositioned("bbbb"))
                .Validate();
            var t = builder.Serialize();
        }

        [Fact]
        public void Test3()
        {
            var builder = new JsonDataBuilder(JsonTestDataType.Auto);
            builder.ForMethod(() => StaticTargetMethod2(null));
            builder.AddCase(x => x
                .AppendPositioned("aaaa")
            );
            builder.Validate();
            var t = builder.Serialize();
        }

        private void TargetMethod1(string testData)
        {
        }

        private void TargetMethod2(string testData, int testInt)
        {
        }

        private static void StaticTargetMethod2(string testData)
        {
        }
    }
}