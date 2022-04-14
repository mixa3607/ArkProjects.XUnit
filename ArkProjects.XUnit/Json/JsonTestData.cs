using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ArkProjects.XUnit.Json
{
    /// <summary>
    /// Data model for <seealso cref="JsonDataAttribute"/>
    /// </summary>
    public class JsonTestData
    {
        /// <summary>
        /// Data type for <see cref="TestCases"/>
        /// </summary>
        public JsonTestDataType DataType { get; set; } = JsonTestDataType.Auto;

        /// <summary>
        /// Test cases array. JToken must be array or dictionary
        /// </summary>
        public IReadOnlyList<JToken> TestCases { get; set; } = Array.Empty<JToken>();
    }
}