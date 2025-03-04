using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RobotCore.Library.Core;
using RobotCore.XmlRpcService;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace RobotCore.StandaloneServer;

internal class Program
{
    public static void Main(string[] args)
    {
        var nlogLogger = new RobotLogger();
        
        // Build and configure the generic host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, logging) =>
            {
                // 1) Clear default providers
                logging.ClearProviders();

                // 2) Decide the minimum level (optional)
                logging.SetMinimumLevel(LogLevel.Trace);

                // 3) Add NLog as the Logging Provider
                //    This picks up your NLog.config or NLog section in appsettings.json automatically
                logging.AddNLog(nlogLogger.Configuration);
            })
            .ConfigureServices((context, services) =>
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
        // Add libraries
        var remoteLibrary = host.Services.GetRequiredService<IKeywordManager>();
        remoteLibrary.AddLibrary(new ExampleLibrary());
        
        // "host.Services" now has an NLog-based ILogger<T>.
        var service = host.Services.GetRequiredService<HttpService>();
        service.Start();

        // Keep your app alive until a key-press or signal, etc.
        Console.WriteLine("Press any key to stop...");
        Console.ReadKey(true);

        service.Stop();
        host.Dispose();
    }
}