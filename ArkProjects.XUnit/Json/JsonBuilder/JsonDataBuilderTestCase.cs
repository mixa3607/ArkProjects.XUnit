using System;
using System.Collections.Generic;

namespace ArkProjects.XUnit.Json.JsonBuilder
{
    public class JsonDataBuilderTestCase
    {
        private List<object?>? _parametersList;
        private Dictionary<string, object?>? _parametersDict;

        internal IReadOnlyDictionary<string, object?>? ParametersDictionary => _parametersDict;
        internal IReadOnlyList<object?>? ParametersList => _parametersList;

        public JsonTestDataType DataType { get; }

        public JsonDataBuilderTestCase(JsonTestDataType dataType)
        {
            DataType = dataType;
        }

        public JsonDataBuilderTestCase AppendPositioned(params object[] parameters)
        {
            if (_parametersDict != null)
            {
                throw new Exception("You can't use named and positioned test case arguments simultaneously");
            }

            if (DataType == JsonTestDataType.SingleParam && parameters.Length + _parametersList?.Count > 1)
            {
                throw new Exception($"You can't set more than 1 argument if {nameof(DataType)} is {JsonTestDataType.SingleParam}");
            }

            _parametersList ??= new List<object?>();
            _parametersList.AddRange(parameters);
            return this;
        }

        public JsonDataBuilderTestCase SetNamed(string parameterName, object? value)
        {
            if (_parametersList != null)
            {
                throw new Exception("You can't use named and positioned test case arguments simultaneously");
            }

            if (DataType == JsonTestDataType.SingleParam && 1 + _parametersList?.Count > 1)
            {
                throw new Exception($"You can't set more than 1 argument if {nameof(DataType)} is {JsonTestDataType.SingleParam}");
            }

            _parametersDict ??= new Dictionary<string, object?>();
            _parametersDict[parameterName] = value;
            return this;
        }
    }
}