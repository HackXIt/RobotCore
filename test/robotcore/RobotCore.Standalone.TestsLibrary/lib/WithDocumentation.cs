using RobotCore.Library.Attributes;

namespace RobotCore.Standalone.TestsLibrary;
#pragma warning disable 1591
[RobotLibrary("Documentation", "Initialization")]
public class WithDocumentation
{
    public void MethodWithoutComments() { }
    
    /// <summary>
    /// This is a method with a comment
    /// </summary>
    /// <param name="arg1">The first argument</param>
    /// <param name="arg2">The second argument</param>
    /// <returns>
    /// The return value
    /// </returns>
    /// <remarks>
    /// This is a developer note
    /// </remarks>
    public void MethodWithComments(string arg1, int arg2) { }
    
    [RobotTags("tag1", "tag2")]
    public void MethodWithTags() { }
    
    [RobotDocumentation("This is a method with documentation")]
    public void MethodWithDocumentation() {  }
    
    [RobotKeyword("This is a method with documentation and tags", "tag1", "tag2")]
    public void MethodWithTagsAndDocumentation() {  }
}
#pragma warning restore 1591