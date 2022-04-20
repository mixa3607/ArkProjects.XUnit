using Newtonsoft.Json;
using Xunit.Abstractions;

namespace ArkProjects.XUnit.Json
{
    public class XUnitJsonSerializable : IXunitSerializable
    {
        public const string XUnitValueKey = "JSON";

        public void Deserialize(IXunitSerializationInfo info)
        {
            var jsonStr = (string)info.GetValue(XUnitValueKey, typeof(string));
            JsonConvert.PopulateObject(jsonStr, this, XUnitJsonSettings.GetJsonSerializerSettings());
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(XUnitValueKey, JsonConvert.SerializeObject(this, XUnitJsonSettings.GetJsonSerializerSettings()));
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, XUnitJsonSettings.GetJsonSerializerSettings());
        }
    }
}