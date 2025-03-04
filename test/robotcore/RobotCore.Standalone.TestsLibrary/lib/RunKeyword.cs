using System.Diagnostics;
using NLog;
using RobotCore.Library.Exceptions;

namespace RobotCore.Standalone.TestsLibrary;

#pragma warning disable 1591

/// <summary>
/// Keywords used by the RunKeywordFixture
/// </summary>
public class RunKeyword
{
    private Logger _logger = LogManager.GetCurrentClassLogger();

    public void NoInputNoOutput() { }

    public void NoError() { }

    public void ThrowsException() {  throw new Exception("A regular exception");}

    public void ThrowsFatalException() { throw new RobotException.FatalKeywordException("A fatal exception"); }

    public void ThrowsContinuableException() { throw new RobotException.ContinuableKeywordException("A continuable exception"); }

    public void WritesTraceOutput()
    {
        Trace.WriteLine("First line");
        Trace.WriteLine("Second line");
    }

    /// <summary>
    /// Keyword used by multithread fixture
    /// </summary>
    public string MultiThreadKeyword(string wait)
    {
        Trace.WriteLine("Waiting");
        Thread.Sleep(Convert.ToInt32(wait));
        return "OK";
    }

    public void WriteAsciiSetToTrace()
    {
        //Trace.WriteLine("0x01: " + 0x01);
        Trace.WriteLine(PrintAsciiCharacters());
    }

    public void WriteAsciiSetToLog()
    {
        _logger.Info(PrintAsciiCharacters());
    }
        
    static string PrintAsciiCharacters()
    {
        var asciiChars = new List<string>();
        for (int i = 1; i < 128; i++)
        {
            asciiChars.Add(((char)i).ToString());
        }
        return string.Join(", ", asciiChars);
    }

}


#pragma warning restore 1591