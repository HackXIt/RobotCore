namespace RobotCore.Library.Attributes;

// NOTE Best-practice tells me to split this into multiple files, but I felt it to be appropriate in this case

public interface IRobotDocumentation
{
    string Documentation { get; }
}

public interface IRobotTags
{
    string[] Tags { get; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RobotDocumentationAttribute : Attribute, IRobotDocumentation
{
    public string Documentation { get; }
    public RobotDocumentationAttribute(string documentation)
    {
        Documentation = documentation;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RobotTagsAttribute : Attribute, IRobotTags
{
    public string[] Tags { get; }
    public RobotTagsAttribute(params string[] tags)
    {
        Tags = tags;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RobotKeywordAttribute : Attribute, IRobotDocumentation, IRobotTags
{
    public string Documentation { get; }
    public string[] Tags { get; }

    public RobotKeywordAttribute(string documentation, params string[] tags)
    {
        Documentation = documentation;
        Tags = tags;
    }
}
