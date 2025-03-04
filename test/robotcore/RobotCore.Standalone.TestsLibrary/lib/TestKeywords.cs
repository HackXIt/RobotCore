using System.Diagnostics;
using RobotCore.Library.Attributes;

namespace RobotCore.Standalone.TestsLibrary;

#pragma warning disable 1591


/// <summary>
/// Class containing different keyword method signatures
/// </summary>
[RobotLibrary("This is a test library", "This is the initialization string")]
public class TestKeywords
{

    #region IntegerReturnTypes


    /// <summary>
    /// Test keyword with return type of int
    /// </summary>
    /// <returns>Returns 1 statically</returns>
    public int Int_ReturnType()
    {
        return 1;
    }

    /// <summary>
    /// Test keyword with return type of Int32
    /// </summary>
    /// <returns>Returns 1 statically</returns>
    public Int32 Int32_ReturnType()
    {
        return 1;
    }

    /// <summary>
    /// Test keyword with return type of long
    /// </summary>
    /// <returns>Returns 1 statically</returns>
    public long Long_ReturnType()
    {
        return 1;
    }
    
    /// <summary>
    /// Test keyword with return type of Int64
    /// </summary>
    /// <returns>Returns 1 statically</returns>
    public Int64 Int64_ReturnType()
    {
        return 1;
    }

    #endregion

    #region StringReturnTypes

    /// <summary>
    /// Test keyword with return type of string using alias
    /// </summary>
    /// <returns>Returns "1" as string</returns>
    public string stringalias_ReturnType()
    {
        return "1";
    }

    /// <summary>
    /// Test keyword with return type of string class
    /// </summary>
    /// <returns>Returns "1" as string</returns>
    public String string_ReturnType()
    {
        return "1";
    }


    #endregion

    #region DoubleReturnTypes

    public double DoubleAlias_ReturnType()
    {
        return 1;
    }

    public double Double_ReturnType()
    {
        return 1;
    }

    #endregion

    #region BooleanReturnTypes

    public bool Boolean_ReturnType()
    {
        return true;
    }

    public bool BooleanAlias_ReturnType()
    {
        return true;
    }

    #endregion

    #region StringArrayReturnType

    public string[] StringArray_ReturnType()
    {
        return new string[] { "1", "2", "3" };
    }

    #endregion

    #region VoidReturnType

    public void Void_ReturnType() {  }

    #endregion

    #region NoParameters

    public void No_Parameters() { }

    #endregion

    #region MethodAccess

    public void Public_Method() { }

    public static void PublicStatic_Method() { }


    #endregion

    #region UnsupportedReturnTypes

    public float Single_ReturnType()
    {
        return 1;
    }

    public decimal Decimal_ReturnType()
    {
        return 1;
    }

    public int[] IntegerArray_ReturnType()
    {
        return new int[] { 1, 2, 3 };
    }

    public double[] DoubleArray_ReturnType()
    {
        return new double[] { 1, 2, 3 };
    }

    public bool[] BooleanArray_ReturnType()
    {
        return new bool[] { true, false };
    }

    #endregion

    #region UnsupportedParameterTypes

    /* These parameters are all supported by now
    public void Integer_ParameterType(int arg1, int arg2) { }

    public void Boolean_ParameterType(bool arg1, bool arg2) { }

    public void Double_ParameterType(double arg1, double arg2) { }

    public void Mixed_ParameterType(string arg1, bool arg2, int arg3, double arg4) { }

    public void StringArrary_ParameterType(string[] arg1) { }
    */

    #endregion

    #region UnSupportedMethodAccess

    private void Private_Method() { }

    internal void Internal_Method() { }

    protected void Protected_Method() { }

    [Obsolete]
    public void Obsolete_Method() { }

    private static void PrivateStatic_Method() { }

    internal static void InternalStatic_Method() { }

    protected static void ProtectedStatic_Method() { }

    #endregion

    #region default_values

    public void Integer_ParameterType_defaultValue(int arg1=1, int arg2=-1) { }

    public void Boolean_ParameterType_defaultValue(bool arg1=true, bool arg2=false) { }

    public void Double_ParameterType_defaultValue(double arg1=1.5, double arg2=-1.5) { }

    public void Mixed_ParameterType_defaultValue(string arg1="Hello", bool arg2=true, int arg3=1, double arg4=1.5) { }
        
    public void Integer2_ParameterType_defaultValue(string required, int arg1=1, int arg2=-1) { }

    public void Boolean2_ParameterType_defaultValue(string required, bool arg1=true, bool arg2=false) { }

    public void Double2_ParameterType_defaultValue(string required, double arg1=1.5, double arg2=-1.5) { }

    public void Mixed2_ParameterType_defaultValue(string required, string arg1="Hello", bool arg2=true, int arg3=1, double arg4=1.5) { }

    #endregion

    #region OptionalParameters

    public string OptionalParameters_Mixed(string arg1, int arg2, double arg3, bool arg4, string arg5 = "optional", int arg6 = -1)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                           $"{nameof(arg2)}={arg2}\n" +
                           $"{nameof(arg3)}={arg3}\n" +
                           $"{nameof(arg4)}={arg4}\n" +
                           $"{nameof(arg5)}={arg5}\n" +
                           $"{nameof(arg6)}={arg6}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }

    #endregion

    #region MultipleParameters

    public string MultipleParameters_Int32(int arg1, int arg2, int arg3, int arg4, int arg5)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                           $"{nameof(arg2)}={arg2}\n" +
                           $"{nameof(arg3)}={arg3}\n" +
                           $"{nameof(arg4)}={arg4}\n" +
                           $"{nameof(arg5)}={arg5}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }
    
    public string MultipleParameters_Boolean(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                           $"{nameof(arg2)}={arg2}\n" +
                           $"{nameof(arg3)}={arg3}\n" +
                           $"{nameof(arg4)}={arg4}\n" +
                           $"{nameof(arg5)}={arg5}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }
    
    public string MultipleParameters_Double(double arg1, double arg2, double arg3, double arg4, double arg5)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                           $"{nameof(arg2)}={arg2}\n" +
                           $"{nameof(arg3)}={arg3}\n" +
                           $"{nameof(arg4)}={arg4}\n" +
                           $"{nameof(arg5)}={arg5}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }
    
    public string MultipleParameters_String(string arg1, string arg2, string arg3, string arg4, string arg5)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                           $"{nameof(arg2)}={arg2}\n" +
                           $"{nameof(arg3)}={arg3}\n" +
                           $"{nameof(arg4)}={arg4}\n" +
                           $"{nameof(arg5)}={arg5}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }
    
    /// <summary>
    /// Combines all provided parameters into a string seperated by newline in the format: "name=value"
    /// </summary>
    /// <param name="arg1">Parameter of type string</param>
    /// <param name="arg2">Parameter of type string</param>
    /// <param name="arg3">Parameter of type string</param>
    /// <param name="arg4">Parameter of type string</param>
    /// <param name="arg5">Parameter of type string</param>
    /// <returns>A string of all provided parameters in the mentioned format above</returns>
    public string MultipleParameters_Mixed(string arg1, int arg2, bool arg3, int arg4, string arg5)
    {
        var actualString = $"{nameof(arg1)}={arg1}\n" +
                             $"{nameof(arg2)}={arg2}\n" +
                             $"{nameof(arg3)}={arg3}\n" +
                             $"{nameof(arg4)}={arg4}\n" +
                             $"{nameof(arg5)}={arg5}";
        Trace.WriteLine($"{actualString}");
        return actualString;
    }

    #endregion

    #region Simple_parameter

    public int Int32_ParameterType(int value)
        => KeywordHelper.OutputObject(value);
    
    public bool Boolean_ParameterType(bool value)
        => KeywordHelper.OutputObject(value);
    
    public double Double_ParameterType(double value)
        => KeywordHelper.OutputObject(value);
    
    public string String_ParameterType(string value)
        => KeywordHelper.OutputObject(value);

    #endregion
    
    #region Complex_parameter
    
    public int[] ArrayInt32_ParameterType(int[] array) 
        => KeywordHelper.OutputArray(array);
    
    public bool[] ArrayBoolean_ParameterType(bool[] array) 
        => KeywordHelper.OutputArray(array);
    
    public double[] ArrayDouble_ParameterType(double[] array) 
        => KeywordHelper.OutputArray(array);
    
    public string[] ArrayString_ParameterType(string[] array) 
        => KeywordHelper.OutputArray(array);

    public List<int> ListInt32_ParameterType(List<int> list) 
        => KeywordHelper.OutputList(list);

    public List<int> ListInt32_ParameterType_Multiple(List<int> list, int other)
    {
        list.Add(other);
        return KeywordHelper.OutputList(list);
    } 
    public List<bool> ListBoolean_ParameterType(List<bool> list) 
        => KeywordHelper.OutputList(list);
    public List<double> ListDouble_ParameterType(List<double> list) 
        => KeywordHelper.OutputList(list);
    public List<string> ListString_ParameterType(List<string> list) 
        => KeywordHelper.OutputList(list);
        
    public List<Dictionary<string, object>> ListDictionaryObject_ParameterType(List<Dictionary<string, object>> list) 
        => KeywordHelper.OutputListDict(list);
    
    public List<Dictionary<string, string>> ListDictionaryString_ParameterType(List<Dictionary<string, string>> list) 
        => KeywordHelper.OutputListDict(list);
    
    public List<Dictionary<string, int>> ListDictionaryInt32_ParameterType(List<Dictionary<string, int>> list)
        => KeywordHelper.OutputListDict(list);
    
    public List<Dictionary<string, bool>> ListDictionaryBoolean_ParameterType(List<Dictionary<string, bool>> list)
        => KeywordHelper.OutputListDict(list);
    
    public List<Dictionary<string, double>> ListDictionaryDouble_ParameterType(List<Dictionary<string, double>> list)
        => KeywordHelper.OutputListDict(list);

    public Dictionary<string, int> DictionaryInt32_ParameterType(Dictionary<string, int> dict) 
        => KeywordHelper.OutputDictionary(dict);
    
    public Dictionary<string, int> DictionaryInt32_ParameterType_Multiple(Dictionary<string, int> dict, int other)
    {
        dict.Add(nameof(other), other);
        return KeywordHelper.OutputDictionary(dict);
    } 
    
    public Dictionary<string, bool> DictionaryBoolean_ParameterType(Dictionary<string, bool> dict) 
        => KeywordHelper.OutputDictionary(dict);
    public Dictionary<string, double> DictionaryDouble_ParameterType(Dictionary<string, double> dict) 
        => KeywordHelper.OutputDictionary(dict);
    public Dictionary<string, string> DictionaryString_ParameterType(Dictionary<string, string> dict) 
        => KeywordHelper.OutputDictionary(dict);
        
    public Dictionary<string, List<object>> DictionaryListObject_ParameterType(Dictionary<string, List<object>> dict) 
        => KeywordHelper.OutputDictList(dict);
    
    public Dictionary<string, List<string>> DictionaryListString_ParameterType(Dictionary<string, List<string>> dict)
        => KeywordHelper.OutputDictList(dict);
    
    public Dictionary<string, List<int>> DictionaryListInt32_ParameterType(Dictionary<string, List<int>> dict)
        => KeywordHelper.OutputDictList(dict);
    
    public Dictionary<string, List<bool>> DictionaryListBoolean_ParameterType(Dictionary<string, List<bool>> dict)
        => KeywordHelper.OutputDictList(dict);
    
    public Dictionary<string, List<double>> DictionaryListDouble_ParameterType(Dictionary<string, List<double>> dict)
        => KeywordHelper.OutputDictList(dict);
    #endregion
}

#pragma warning restore 1591