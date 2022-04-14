using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArkProjects.XUnit.Json
{
    public static class XUnitJsonSettings
    {
        private static JsonSerializerSettings? _jsonSerializerSettings;

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            if (_jsonSerializerSettings != null)
            {
                return _jsonSerializerSettings;
            }

            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Formatting = Formatting.Indented;
            _jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            return _jsonSerializerSettings;
        }

        public static readonly JsonSerializer Serializer = JsonSerializer.Create(GetJsonSerializerSettings());
    }
}