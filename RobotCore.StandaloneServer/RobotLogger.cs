using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;

namespace RobotCore.StandaloneServer;

public class RobotLogger
{
    public LoggingConfiguration Configuration;

    public RobotLogger()
    {
        Configuration = new LoggingConfiguration();
        // NOTE TraceTarget RawWrite must be set to true, see: https://stackoverflow.com/questions/73988323/nlog-info-level-outputs-differently-than-debug-level 
        var traceTarget = new TraceTarget("robotLog") { RawWrite = true };
        LayoutRenderer.Register<UnixEpochLayoutRenderer>("unixepoch");
        LayoutRenderer.Register<GenericLoggerNameLayoutRenderer>("genericLogger");
        traceTarget.Layout = "*${level:uppercase=true}:${unixepoch}* ${genericLogger:shortName=true} ${message:withexception=true}";
        //LayoutRenderer.Register<RobotLayoutRenderer>("RobotLayout");
        Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, traceTarget);
        LogManager.Configuration = Configuration;
    }
}