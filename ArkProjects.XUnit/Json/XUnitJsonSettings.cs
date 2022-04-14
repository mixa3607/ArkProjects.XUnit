using Newtonsoft.Json;

namespace ArkProjects.XUnit.Json
{
    public static class XUnitJsonSettings
    {
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
        public static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);
    }
}