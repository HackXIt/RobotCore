using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

namespace RobotCore.XmlRpcService;

/// <summary>
/// An HTTP service for handling RobotFramework requests
/// </summary>
public class HttpService
{
    /// <summary>
    /// NLog instance
    /// </summary>
    private readonly ILogger<HttpService> _logger;

    /// <summary>
    /// HttpListener for handling HTTP-Requests
    /// </summary>
    /// <remarks> Throws "Access denied" when started as user
    /// Requires administrative privileges or activation in URL-AccessControlList for user
    /// Workaround for regular users:
    /// As administrator => `netsh http add urlacl url=http://*:8270/ user=Jeder`
    /// The url in the above command must be equal to the Prefix added in the ctor
    /// Src: https://stackoverflow.com/questions/4019466/httplistener-access-denied
    /// </remarks>
    private readonly HttpListener _listener;
    /// <summary>
    /// HTTP client for sending internal requests to listener
    /// </summary>
    /// <remarks>
    /// Check out the following links for further information about usage:
    /// https://stackoverflow.com/questions/4015324/send-http-post-request-in-net
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
    /// </remarks>
    private static readonly HttpClient Client = new HttpClient();
    private string ListenerUri => $"http://127.0.0.1:{_port}/";
    private Thread? _httpThread;
    private IXmlRpcRequestHandler _rpcHandler;
    private readonly uint _port;
    private bool _started;
    private static volatile bool _keepListenerRunning = true;

    /// <summary>
    /// ctor for HttpService
    /// </summary>
    /// <param name="logger">_logger instance</param>
    /// <param name="rpcHandler">Instance of XmlRpcService for handling RobotFramework</param>
    /// <param name="port">TCP Port to use for HTTP listener</param>
    public HttpService(ILogger<HttpService> logger, IXmlRpcRequestHandler rpcHandler, uint port = 8270)
    {
        _logger = logger;
        _rpcHandler = rpcHandler;
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{_port}/");
        _httpThread = null;
    }

    /// <summary>
    /// Start HTTP listener in new Thread
    /// </summary>
    /// <exception cref="SystemException">Throws exception, if given TCP port is unavailable</exception>
    public void Start()
    {
        if (_started) return;
        if (PortUnavailable())
            throw new SystemException($"Unable to start HTTP listener service, port {_port} already in use.");
        _httpThread = new Thread(ListenerTask)
        {
            IsBackground = true
        };
        _keepListenerRunning = true;
        _httpThread.Start();
        _started = true;
    }

    /// <summary>
    /// Send DELETE request to HTTP listener to stop the thread 
    /// </summary>
    /// <exception cref="SystemException">Throws exception, if listener doesn't respond or gives back NOK status</exception>
    public async void Stop()
    {
        if (!_started) return;
        _logger.LogInformation("Sending HTTP request to stop service");
        var response = await Client.DeleteAsync(ListenerUri);
        if (!response.IsSuccessStatusCode)
            throw new SystemException("Failed to stop HTTP listener service");
        _started = false;
    }
        
    /// <summary>
    /// Check if given TCP port is not available for usage
    /// </summary>
    /// <returns>True if port is unavailable, false if port is available</returns>
    private bool PortUnavailable()
    {
        // NOTE Info on checking TCP-Ports: https://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        // Check if any connectionInformation.port is equal to our port, if not then port is available
        return tcpConnInfoArray.Any(connection => connection.LocalEndPoint.Port == _port);
    }

    #region HTTP_Listener

    /// <summary>
    /// Internal implementation of Task for HTTP listener thread.
    /// </summary>
    private void ListenerTask()
    {
        _logger.LogInformation($"HTTP listener started on port {_port}");
        _listener.Start();
        while (_keepListenerRunning)
        {
            try
            {
                // FIXME Improve Exception handling for HTTP listener
                var requestContext = _listener.GetContext(); // Wait for HTTP message
                var method = requestContext.Request.HttpMethod;
                _logger.LogDebug($"Received HTTP request with method {method}");
                /* INFO switch-comparison with string uses String.Equals()
                Source: https://stackoverflow.com/questions/57544979/which-string-comparer-is-used-with-switch-statements
                */
                switch (method)
                {
                    case "POST":
                        Task.Factory.StartNew(() => ProcessRequest(requestContext));
                        break;
                    case "DELETE":
                        _logger.LogDebug($"Closing http listener since DELETE was received");
                        _keepListenerRunning = false;
                        requestContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        // TODO Check if there's anything else to provide or do here
                        requestContext.Response.Close();
                        break;
                    default:
                        _logger.LogDebug($"Unhandled http request method '{method}' received");
                        requestContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                        requestContext.Response.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP listener thread exited unexpectedly");
                _keepListenerRunning = false;
            }
        }
        _logger.LogInformation($"HTTP listener thread has exited");
    }
        
    #region HTTP_Responses
        
    /// <summary>
    /// Process received POST request (Forward to XmlRpc-Service for handling RobotFramework Library)
    /// </summary>
    /// <param name="requestContext">HTTP POST request context from listener</param>
    private void ProcessRequest(HttpListenerContext requestContext)
    {
        _logger.LogDebug($"Processing HTTP request for URL: {requestContext.Request.Url}");
        try
        {
            // Forward request to XmlRpc RemoteService
            _rpcHandler.ProcessRequest(requestContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing HTTP POST request: {ex.Message}");
            requestContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            requestContext.Response.StatusDescription = ex.Message;
            requestContext.Response.Close();
        }
    }

    #endregion

    #endregion
}