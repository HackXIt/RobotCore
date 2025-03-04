using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using RobotCore.Library.Core;
using RobotCore.StandaloneServer;
using RobotCore.XmlRpcService;

namespace RobotCore.Standalone.TestsLibrary;

internal class Program
{
    private static IHost? _host;
    private static HttpService? _service;
    
    public static void Main(string[] args)
    {
        // 1) Create your custom NLog configuration for the tests
            var nlogConfig = new LoggingConfiguration();

            // Add the same targets/rules you had in your test class
            var consoleTarget = new ConsoleTarget("consoleLog");
            var traceTarget = new TraceTarget("traceLog");

            nlogConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
            nlogConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, traceTarget);

            // 2) Build the Host (similar to Program.Main)
            //    but use the test-specific NLog config
            _host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddNLog(nlogConfig); // Use your in-memory config
                })
                .ConfigureServices(services =>
                {
                    // Register RobotRemoteLibrary
                    services.AddSingleton<RobotRemoteLibrary>();
                    // Register all services
                    services.AddSingleton<HttpService>();
                    services.AddSingleton<IXmlRpcRequestHandler>(sp => sp.GetRequiredService<RobotRemoteLibrary>());
                    services.AddSingleton<IRobotFrameworkRemoteApi>(sp => sp.GetRequiredService<RobotRemoteLibrary>());
                    services.AddSingleton<IKeywordManager, KeywordManager>();
                })
                .Build();
            
            // 3) Add libraries
            var remoteLibrary = _host.Services.GetRequiredService<IKeywordManager>();

            // 4) Retrieve your service and start it
            _service = _host.Services.GetRequiredService<HttpService>();
            _service.Start();
            
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);
            
            // 5) Stop the service and dispose of the host
            _service.Stop();
            _host.Dispose();
    }
}