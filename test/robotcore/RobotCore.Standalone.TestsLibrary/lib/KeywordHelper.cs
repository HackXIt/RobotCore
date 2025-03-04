using System.Diagnostics;

namespace RobotCore.Standalone.TestsLibrary;

public static class KeywordHelper
{
    public static T OutputObject<T>(T obj)
    {
        if (obj is null)
        {
            Trace.WriteLine("null");
            return obj;
        }
        Trace.WriteLine($"{obj}({obj.GetType()})");
        return obj;
    }
    
    public static T[] OutputArray<T>(T[] array)
    {
        var i = 0;
        foreach (var value in array)
        {
            Trace.WriteLine($"[{i}]={value}");
            i++;
        }

        return array;
    }
        
    public static List<T> OutputList<T>(List<T> list)
    {
        var i = 0;
        foreach (var value in list)
        {
            Trace.WriteLine($"[{i}]={value}");
            i++;
        }

        return list;
    }

    public static List<Dictionary<string, T>> OutputListDict<T>(List<Dictionary<string, T>> listDict)
    {
        var i = 0;
        foreach (var entry in listDict)
        {
            Trace.WriteLine($"[{i}]: ...");
            OutputDictionary(entry);
            i++;
        }

        return listDict;
    }
    
    

    public static Dictionary<string, T> OutputDictionary<T>(Dictionary<string, T> dict)
    {
        var i = 0;
        foreach (var entry in dict)
        {
            Trace.WriteLine($"[{i}]={{key={entry.Key}, value={entry.Value}}}");
            i++;
        }

        return dict;
    }
    
    public static Dictionary<string, List<T>> OutputDictList<T>(Dictionary<string, List<T>> dictList)
    {
        var i = 0;
        foreach (var entry in dictList)
        {
            Trace.WriteLine($"[{i}]={{key={entry.Key}, value=...}}");
            OutputList(entry.Value);
            i++;
        }

        return dictList;
    }
}