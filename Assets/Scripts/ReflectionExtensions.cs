using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

public static class ReflectionExtensions
{

    public static JObject GetJsonSchema(this object obj, string methodName)
    {
        MethodInfo method = obj.GetType().GetMethod(methodName);

        if (method == null)
        {
            return null;
        }

        return method.GenerateJsonSchema();
    }

    public static JObject GenerateJsonSchema(this Type type)
    {
        var jObject = new JObject();
        foreach (var property in type.GetProperties())
        {
            var propertyType = property.PropertyType;
            jObject.Add(property.Name, GetJsonType(propertyType));
        }
        return jObject;
    }

    public static JObject GenerateJsonSchema(this MethodInfo method)
    {
        var jObject = new JObject
        {
            ["description"] = method.Name,
            ["type"] = "object",
            ["properties"] = new JObject(),
            ["required"] = new JArray()
        };

        foreach (var parameter in method.GetParameters())
        {
            ((JObject)jObject["properties"]).Add(parameter.Name, GetJsonType(parameter.ParameterType));

            if (!parameter.HasDefaultValue)
            {
                ((JArray)jObject["required"]).Add(parameter.Name);
            }
        }

        jObject["returns"] = GetJsonType(method.ReturnType);

        return jObject;
    }


    public static JToken GetJsonType(this Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        var typeCode = Type.GetTypeCode(underlyingType);

        switch (typeCode)
        {
            case TypeCode.Boolean: return "boolean";
            case TypeCode.Char: return "string";
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal: return "number";
            case TypeCode.DateTime: return "string"; // Should ideally be "string" with a "format" of "date-time"
            case TypeCode.String: return "string";
            default:
                return underlyingType.IsEnum ? "integer" : (underlyingType.IsArray ? "array" : (underlyingType.IsClass ? "object" : "null"));
        }
    }
}