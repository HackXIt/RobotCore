using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace RobotCore.StandaloneServer;

/// <summary>
/// A type name renderer which either logs the 
/// </summary>
[LayoutRenderer("genericLogger")]
public class GenericLoggerNameLayoutRenderer : LoggerNameLayoutRenderer
{
    private const char Backtick = '`';
    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        base.Append(builder, logEvent);
        var backtickIndex = TryGetBacktickForGenericType(logEvent);
        if (backtickIndex < 0) return;
        // ReSharper disable once AssignNullToNotNullAttribute
        // (LoggerName can't be null after checking for backtick)
        var type = Type.GetType(logEvent.LoggerName); // Get type for generic arguments
        if (type is not { IsGenericType: true }) return; // No loadable or generic type
        // Remove backtick with trailing number from builder and replace with type arguments
        builder.Remove(backtickIndex, builder.Length - backtickIndex);
        builder.Append('<');
        var arguments = type.GetGenericArguments().Select(t => t.Name).ToArray();
        builder.Append(string.Join(", ", arguments));
        builder.Append('>');
    }
        
    private int TryGetBacktickForGenericType(LogEventInfo logEvent)
    {
        return logEvent.LoggerName?.LastIndexOf(Backtick) ?? -1;
    }
}