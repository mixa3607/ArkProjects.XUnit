using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace ArkProjects.XUnit.Json
{
    [DataDiscoverer("ArkProjects.XUnit.Json." + nameof(JsonDataDiscoverer), "ArkProjects.XUnit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JsonDataAttribute : DataAttribute
    {
        private readonly string _path;

        /// <summary>
        /// Returns <c>true</c> if the data attribute wants to skip enumerating data during discovery.
        /// This will cause the theory to yield a single test case for all data, and the data discovery
        /// will be during test execution instead of discovery.
        /// </summary>
        public bool DisableDiscoveryEnumeration { get; set; } = false;

        public JsonDataAttribute(string path)
        {
            _path = path;
        }

        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            var path = JsonDataHelper.PreparePath(_path, testMethod);

            var jsonStr = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<JsonTestData>(jsonStr, XUnitJsonSettings.SerializerSettings);
            if (json == null)
                throw new InvalidDataException("Json deserialized as null");

            var attrDict = new Dictionary<Type, object>()
            {
                { typeof(XUnitJsonFileNameAttribute), path }
            };
            return JsonDataHelper.ExtractData(json, testMethod.GetParameters(), attrDict);
        }
    }
}