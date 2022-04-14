using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArkProjects.XUnit.Json.JsonBuilder
{
    public class JsonDataBuilder
    {
        private readonly List<JsonDataBuilderTestCase> _testCases;
        private MethodInfo? _targetMethod;
        private JsonTestData? _buildDataCache;

        public JsonTestDataType DataType { get; }

        public JsonDataBuilder(JsonTestDataType dataType = JsonTestDataType.Auto)
        {
            DataType = dataType;
            _testCases = new List<JsonDataBuilderTestCase>();
        }

        public JsonDataBuilder AddCase(Action<JsonDataBuilderTestCase> cfg)
        {
            var tCase = new JsonDataBuilderTestCase(DataType);
            cfg(tCase);
            AddCaseInternal(tCase);
            return this;
        }

        public JsonDataBuilder ForMethod<T>(Expression<Action<T>> methodSelector)
        {
            if (methodSelector.NodeType != ExpressionType.Call)
            {
                throw new Exception("Support only Call expressions");
            }

            var callExp = (MethodCallExpression)methodSelector.Body;
            _targetMethod = callExp.Method;
            return this;
        }

        public JsonTestData Build()
        {
            if (_buildDataCache != null)
            {
                return _buildDataCache;
            }

            var testCases = new List<object>();
            foreach (var testCase in _testCases)
            {
                object testCaseObj;
                if (DataType == JsonTestDataType.Auto)
                {
                    if (_testCases.All(x => x.ParametersDictionary?.Count == 1 || x.ParametersList?.Count == 1))
                    {
                        testCaseObj = (testCase.ParametersDictionary?.Values.First() ?? testCase.ParametersList?.First())!;
                    }
                    else
                    {
                        testCaseObj = ((object?)testCase.ParametersDictionary ?? testCase.ParametersList)!;
                    }
                }
                else if (DataType == JsonTestDataType.MultiParams)
                {
                    testCaseObj = ((object?)testCase.ParametersDictionary ?? testCase.ParametersList)!;
                }
                else if (DataType == JsonTestDataType.SingleParam)
                {
                    testCaseObj = (testCase.ParametersDictionary?.Values.First() ?? testCase.ParametersList?.First())!;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(DataType)} {DataType} not supported");
                }

                testCases.Add(testCaseObj);
            }

            var result = new JsonTestData()
            {
                DataType = DataType,
                TestCases = testCases.Select(x => JToken.FromObject(x, XUnitJsonSettings.Serializer)).ToArray()
            };
            _buildDataCache = result;
            return result;
        }

        public void Validate()
        {
            if (_targetMethod == null)
            {
                throw new Exception($"Before call {nameof(Validate)} must set target method");
            }
            var data = Build();

            JsonDataHelper.ExtractData(data, _targetMethod.GetParameters(), new Dictionary<Type, object>());
        }

        public void Save(string outPath)
        {
            var data = Build();
            var jsonStr = JsonConvert.SerializeObject(data, XUnitJsonSettings.SerializerSettings);
            File.WriteAllText(outPath, jsonStr);
        }

        private void AddCaseInternal(JsonDataBuilderTestCase testCase)
        {
            if (DataType != JsonTestDataType.Auto && testCase.DataType != DataType)
            {
                throw new Exception($"Builder {nameof(DataType)} must be {JsonTestDataType.Auto} or equal {nameof(testCase.DataType)} in test case");
            }

            if (testCase.ParametersDictionary?.Count >= 1 || testCase.ParametersList?.Count >= 1)
            {
                _buildDataCache = null;
                _testCases.Add(testCase);
            }
            else
            {
                throw new Exception("Test case must contain one and more values");
            }
        }
    }
}