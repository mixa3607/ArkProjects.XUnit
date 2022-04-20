using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ArkProjects.XUnit.Json
{
    internal static class JsonDataHelper
    {
        internal static IReadOnlyList<object?[]> ExtractData(JsonTestData jsonData, ParameterInfo[] parameters, Dictionary<Type, object> valuesByAttr)
        {
            if (parameters.Length == 0)
            {
                return Array.Empty<object[]>();
            }

            var jTestCases = jsonData.TestCases;
            switch (jsonData.DataType)
            {
                case JsonTestDataType.Auto:
                    try
                    {
                        return parameters.Length == 1
                            ? ExtractSingle(jTestCases, parameters[0], valuesByAttr)
                            : ExtractMulti(jTestCases, parameters, valuesByAttr);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidDataException($"Can't read data in {jsonData.DataType} mode. You can force it (see docs)", e);
                    }
                case JsonTestDataType.MultiParams:
                    try
                    {
                        return ExtractMulti(jTestCases, parameters, valuesByAttr);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidDataException($"Can't read data in {jsonData.DataType} mode (forced)", e);
                    }
                case JsonTestDataType.SingleParam:
                    try
                    {
                        return ExtractSingle(jTestCases, parameters[0], valuesByAttr);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidDataException($"Can't read data in {jsonData.DataType} mode (forced)", e);
                    }
                default:
                    throw new NotSupportedException($"Mode {jsonData.DataType} not supported");
            }
        }

        internal static IReadOnlyList<object?[]> ExtractMulti(IReadOnlyList<JToken> jTokens, ParameterInfo[] parameters, Dictionary<Type, object> valuesByAttr)
        {
            var testCases = new List<object?[]>();
            for (var i = 0; i < jTokens.Count; i++)
            {
                var jToken = jTokens[i];
                valuesByAttr[typeof(XUnitJsonCaseIndexAttribute)] = i;
                switch (jToken)
                {
                    case JObject jObject:
                    {
                        var values = parameters.Select(x =>
                                jObject.TryGetValue(x.Name, StringComparison.InvariantCultureIgnoreCase, out var value)
                                    ? value.ToObject(x.ParameterType, XUnitJsonSettings.Serializer)
                                    : null)
                            .Select(x =>
                            {
                                SetValuesByAttrs(x, valuesByAttr);
                                return x;
                            })
                            .ToArray();
                        testCases.Add(values);
                        break;
                    }
                    case JArray jArray:
                    {
                        var values = parameters
                            .Select((t, i) => jArray.Count > i ? jArray[i].ToObject(t.ParameterType, XUnitJsonSettings.Serializer) : null)
                            .Select(x =>
                            {
                                SetValuesByAttrs(x, valuesByAttr);
                                return x;
                            })
                            .ToList();
                        testCases.Add(values.ToArray());
                        break;
                    }
                    default:
                        throw new InvalidDataException($"Token must be array or object but read {jToken.Type}");
                }
            }

            return testCases;
        }

        internal static IReadOnlyList<object?[]> ExtractSingle(IReadOnlyList<JToken> jTokens, ParameterInfo parameter, Dictionary<Type, object> valuesByAttr)
        {
            return jTokens.Select((x, i) =>
            {
                valuesByAttr[typeof(XUnitJsonCaseIndexAttribute)] = i;
                var obj = x?.ToObject(parameter.ParameterType, XUnitJsonSettings.Serializer);
                SetValuesByAttrs(obj, valuesByAttr);
                return new[] { obj };
            }).ToArray();
        }


        internal static string PreparePath(string rawPath, MethodInfo testMethod)
        {
            var path = rawPath
                    .Replace("{class}", testMethod.ReflectedType?.Name)
                    .Replace("{method}", testMethod.Name)
                ;
            return path;
        }

        private static void SetValuesByAttrs(object? obj, IReadOnlyDictionary<Type, object> valuesByAttr)
        {
            if (obj == null || !obj.GetType().IsClass)
            {
                return;
            }

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var customAttrs = propertyInfo.GetCustomAttributes();
                foreach (var customAttr in customAttrs)
                {
                    if (valuesByAttr.TryGetValue(customAttr.GetType(), out var val))
                    {
                        object? propVal = null; 
                        var typeUnderNullable = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                        if (typeUnderNullable != null)
                        {
                            propVal = Convert.ChangeType(val, typeUnderNullable);
                        }
                        else
                        {
                            propVal = Convert.ChangeType(val, propertyInfo.PropertyType);
                        }
                        propertyInfo.SetValue(obj, propVal);
                    }
                }
            }
        }
    }
}