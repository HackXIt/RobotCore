using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace RobotCore.StandaloneServer;

[LayoutRenderer("unix-epoch")]
public class UnixEpochLayoutRenderer : LayoutRenderer
{
    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        builder.Append($"{new DateTimeOffset(logEvent.TimeStamp).ToUnixTimeMilliseconds()}");
    }
}