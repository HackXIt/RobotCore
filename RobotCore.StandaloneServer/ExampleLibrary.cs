using System.Diagnostics;

namespace RobotCore.StandaloneServer;

public class ExampleLibrary
{
    public string WriteString(string text)
    {
        Trace.WriteLine(text);
        return text;
    }
}