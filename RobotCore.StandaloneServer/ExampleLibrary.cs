using System.Diagnostics;

namespace RobotCore.StandaloneServer;

public class ExampleLibrary
{
    public string WriteString(string text, int year)
    {
        Trace.WriteLine($"WriteString: {text} {year}");
        return text;
    }
}