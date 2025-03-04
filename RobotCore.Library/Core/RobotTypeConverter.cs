using Horizon.XmlRpc.Core; // Only if you use XmlRpcStruct or XmlRpcBinary from that library
using System.Collections;
using System.Security.Cryptography;

public class RobotTypeConverter
{
    private const string None = "None";

    /// <summary>
    /// Converts a C# value to an equivalent Robot Framework type.
    /// 
    /// Rules:
    /// - Null => empty string
    /// - string, int, double, bool => return as-is
    /// - byte[] => can be converted to a base64 or an XmlRpcBinary (if you're using Horizon.XmlRpc)
    /// - IEnumerable (except string/dict) => convert elements recursively to list
    /// - IDictionary => convert keys to string, values recursively
    /// - Everything else => ToString()
    /// </summary>
    public static object? ConvertObjectToRobotFramework(object? value)
    {
        if (value == null)
        {
            // Robot treats Python None as empty string
            return string.Empty;
        }

        // Base cases
        switch (value)
        {
            case string s:
                // If you need to handle null bytes or non-XML-safe chars, you’d do that here.
                return s;

            case int i:
                return i; // pass as is

            case double d:
                return d; // pass as is

            case bool b:
                return b; // pass as is

            // TODO Implement XmlRpcBinary conversion if needed
            // case byte[] bytes:
            //     // Robot can’t handle raw bytes that contain e.g. 0x00 in normal XML-RPC strings.
            //     // You could either return a base64 string or an XmlRpcBinary if you’re using Horizon.
            //     // e.g.:
            //     return 
        }

        var type = value.GetType();

        // If it's an IDictionary => convert to Robot dict
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var dict = (IDictionary)value;
            var convertedDict = new Dictionary<string, object?>();
            foreach (DictionaryEntry entry in dict)
            {
                var keyString = entry.Key?.ToString() ?? string.Empty; // Robot keys are always strings
                convertedDict[keyString] = ConvertObjectToRobotFramework(entry.Value);
            }
            return convertedDict;
        }

        // If it's an IEnumerable, but not a string, treat as a list
        if (value is IEnumerable enumerable && value is not string)
        {
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(ConvertObjectToRobotFramework(item));
            }
            return list;
        }

        // Otherwise, just convert to string
        return value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Converts a Robot Framework value (as it arrives into .NET) into an equivalent C# type.
    /// 
    /// The <paramref name="targetType"/> indicates which .NET type you ultimately want.
    /// - If <paramref name="value"/> is 'None' or null, handle accordingly.
    /// - If <paramref name="targetType"/> is string/int/double/bool/etc., parse or cast.
    /// - If <paramref name="targetType"/> is an array/list/dict, convert recursively.
    /// - If we can’t handle it, fallback to string or null.
    /// </summary>
    public static object? ConvertObjectFromRobotFramework(object? value, Type targetType)
    {
        // Handle "None" or null => either null or empty string, depending on the target type
        if (value == null || value as string == None)
        {
            // If the target is string, return empty; else return null.
            if (targetType == typeof(string))
                return string.Empty;
            // For reference types, null is safe to return. For value types, might do default(...)
            if (!targetType.IsValueType)
                return null;
            
            // If it's a struct or value type, we might do a default instance
            return Activator.CreateInstance(targetType);
        }

        // If the target is string => toString
        if (targetType == typeof(string))
        {
            return value.ToString();
        }
        // If the target is int => try parse
        if (targetType == typeof(int))
        {
            if (value is int iVal) return iVal;
            if (int.TryParse(value.ToString(), out var iParsed))
                return iParsed;
            // fallback
            return 0;
        }
        // If the target is double => try parse
        if (targetType == typeof(double))
        {
            if (value is double dVal) return dVal;
            if (double.TryParse(value.ToString(), out var dParsed))
                return dParsed;
            // fallback
            return 0.0;
        }
        // If the target is bool => try parse
        if (targetType == typeof(bool))
        {
            if (value is bool bVal) return bVal;
            if (bool.TryParse(value.ToString(), out var bParsed))
                return bParsed;
            // Robot might pass "True"/"False" or "1"/"0" so we might do additional checks
            if (value.ToString() == "1") return true;
            if (value.ToString() == "0") return false;
            // fallback
            return false;
        }

        // If it's an array -> we want to fill that array from a Robot list
        if (targetType.IsArray)
        {
            if (value is IList listVal)
            {
                var elemType = targetType.GetElementType() ?? typeof(object);
                var array = Array.CreateInstance(elemType, listVal.Count);
                for (int idx = 0; idx < listVal.Count; idx++)
                {
                    var converted = ConvertObjectFromRobotFramework(listVal[idx], elemType);
                    array.SetValue(converted, idx);
                }
                return array;
            }
            // fallback: empty array
            return Array.CreateInstance(targetType.GetElementType() ?? typeof(object), 0);
        }

        // If it’s a generic List<T>
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var listItemType = targetType.GetGenericArguments()[0];
            var listInstance = Activator.CreateInstance(targetType);
            if (listInstance is IList listObj && value is IList srcList)
            {
                foreach (var item in srcList)
                {
                    var converted = ConvertObjectFromRobotFramework(item, listItemType);
                    listObj.Add(converted);
                }
            }
            return listInstance;
        }

        // If it’s a Dictionary<TKey, TValue>
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = targetType.GetGenericArguments()[0];
            var valType = targetType.GetGenericArguments()[1];
            var dictInstance = Activator.CreateInstance(targetType);
            if (dictInstance is IDictionary dictObj && value is IDictionary srcDict)
            {
                foreach (DictionaryEntry entry in srcDict)
                {
                    // Robot keys are typically string, so convert the key to TKey
                    var keyConverted = ConvertObjectFromRobotFramework(entry.Key, keyType);
                    var valConverted = ConvertObjectFromRobotFramework(entry.Value, valType);
                    dictObj[keyConverted ?? ""] = valConverted;
                }
            }
            return dictInstance;
        }

        // TODO Implement XmlRpcBinary conversion if needed
        // If we have an XmlRpcBinary from Robot, convert to byte[]
        // if (value is XmlRpcBinary xmlRpcBin && targetType == typeof(byte[]))
        // {
        //     return xmlRpcBin.Data;
        // }

        // Fallback => just do string => try parse => or create default
        // But typically let's just return the string. 
        return value.ToString();
    }

    /// <summary>
    /// Converts type information from Robot Framework's type string to a .NET type.
    /// Examples: "int" => typeof(int), "list" => typeof(List&lt;dynamic&gt;), etc.
    /// If it doesn't match known patterns, defaults to string.
    /// </summary>
    public static Type ConvertTypeFromRobotFramework(string type)
    {
        return type.ToLower() switch
        {
            "string" => typeof(string),
            "int" or "integer" => typeof(int),
            "float" or "double" => typeof(double),
            "bool" or "boolean" => typeof(bool),
            "list" =>
                // Minimal default, if Robot says "list", we treat it as List<dynamic>.
                typeof(List<dynamic>),
            "dict" =>
                // Minimal default for dict => Dictionary<string, dynamic>.
                typeof(Dictionary<string, dynamic>),
            "none" => null, // means no particular type => or "None"
            _ => typeof(string)
        } ?? typeof(string);
    }

    /// <summary>
    /// Returns a Robot Framework type-string that corresponds to a .NET type.
    /// (Already filled in from your partial code.)
    /// </summary>
    public static string ConvertTypeToRobotFramework(Type type)
    {
        // Basic approach. Could be expanded with reflection for arbitrary nestings.
        return type switch
        {
            not null when type == typeof(string) => "string",
            not null when type == typeof(int) => "int",
            not null when type == typeof(double) => "double",
            not null when type == typeof(bool) => "boolean",
            not null when type == typeof(string[]) => "list[string]",
            not null when type == typeof(int[]) => "list[int]",
            not null when type == typeof(double[]) => "list[double]",
            not null when type == typeof(bool[]) => "list[boolean]",
            not null when type == typeof(List<bool>) => "list[boolean]",
            not null when type == typeof(List<int>) => "list[int]",
            not null when type == typeof(List<double>) => "list[double]",
            not null when type == typeof(List<string>) => "list[string]",
            not null when type == typeof(List<Dictionary<string, bool>>) => "list[dict[string, boolean]]",
            not null when type == typeof(List<Dictionary<string, int>>) => "list[dict[string, int]]",
            not null when type == typeof(List<Dictionary<string, double>>) => "list[dict[string, double]]",
            not null when type == typeof(List<Dictionary<string, string>>) => "list[dict[string, string]]",
            not null when type == typeof(Dictionary<string, bool>) => "dict[string, boolean]",
            not null when type == typeof(Dictionary<string, int>) => "dict[string, int]",
            not null when type == typeof(Dictionary<string, double>) => "dict[string, double]",
            not null when type == typeof(Dictionary<string, string>) => "dict[string, string]",
            not null when type == typeof(Dictionary<string, List<bool>>) => "dict[string, list[boolean]]",
            not null when type == typeof(Dictionary<string, List<int>>) => "dict[string, list[int]]",
            not null when type == typeof(Dictionary<string, List<double>>) => "dict[string, list[double]]",
            not null when type == typeof(Dictionary<string, List<string>>) => "dict[string, list[string]]",
            _ => "None"
        };
    }

    /// <summary>
    /// Determines whether the given <paramref name="type"/> is "valid" under
    /// the Robot Framework <-> .NET rules we want to support.
    /// </summary>
    /// <remarks>
    /// Arguments and return types of methods in libraries will need to pass this check to be registered as a keyword.
    /// </remarks>
    public static bool IsValidType(Type type)
    {
        // Simple checks for primitives and arrays, or up to 2-level nestings of List/Dict
        if (type == typeof(string)
            || type == typeof(int)
            || type == typeof(double)
            || type == typeof(bool)
            || type == typeof(void))
            // || type == typeof(byte[]))
        {
            return true;
        }

        // Arrays
        if (type.IsArray)
        {
            // Check element type is valid
            var elemType = type.GetElementType();
            return elemType != null && IsValidType(elemType);
        }

        switch (type.IsGenericType)
        {
            // List<T>
            case true when type.GetGenericTypeDefinition() == typeof(List<>):
            {
                var innerType = type.GetGenericArguments()[0];
                return IsValidType(innerType);
            }
            // Dictionary<TKey, TValue>
            case true when type.GetGenericTypeDefinition() == typeof(Dictionary<,>):
            {
                var keyType = type.GetGenericArguments()[0];
                var valType = type.GetGenericArguments()[1];

                // Robot typically expects string keys
                return keyType == typeof(string) && IsValidType(valType);
            }
            default:
                // Otherwise, we might treat all other custom types as invalid or fallback
                return false;
        }
    }
}
