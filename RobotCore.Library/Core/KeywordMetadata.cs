using System.Reflection;

namespace RobotCore.Library.Core;

public class KeywordMetadata
{
    public string Name { get; set; }
    public MethodInfo Method { get; set; }
    public ParameterInfo[] Parameters { get; set; }
    
    public Dictionary<string, (Type type, object? defaultValue, bool IsOptional)> Arguments { get; } = new();
    
    public Dictionary<string, Type> RequiredArguments 
        => Arguments.Where(a => !a.Value.IsOptional).ToDictionary(a => a.Key, a => a.Value.type);
    
    public Dictionary<string, (Type type, object obj)> OptionalArguments
        => Arguments.Where(a => a.Value is { IsOptional: true, defaultValue: not null }).ToDictionary(a => a.Key, a => (a.Value.type, a.Value.defaultValue!));
    
    public List<string> ArgumentNames => Arguments.Keys.ToList();
    
    public Type[] ArgumentTypes => Arguments.Values.Select(a => a.type).ToArray();

    public bool HasOptionalParameters => Parameters.Any(p => p.IsOptional);
    
    public int ArgumentCount => Arguments.Count;
    
    public KeywordMetadata(MethodInfo method)
    {
        Name = method.Name;
        Method = method;
        Parameters = method.GetParameters();
        foreach (var pi in Parameters)
        {
            if (pi.Name != null)
            {
                Arguments.Add(pi.Name, (type: pi.ParameterType, defaultValue: pi.DefaultValue, pi.IsOptional));
            }
        }
    }
}