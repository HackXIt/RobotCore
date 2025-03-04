using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RobotCore.Library.Core;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RobotCore.XmlRpcService;

namespace RobotCore.Service.Controllers;

[ApiController]
[Route("xmlrpc")]
public class XmlRpcController : ControllerBase
{
    private readonly XmlRpcServer _xmlRpcServer;
    private readonly ILogger<XmlRpcController> _logger;

    public XmlRpcController(RobotLibraryBase robotLibrary, ILogger<XmlRpcController> logger)
    {
        _logger = logger;
        // You can instantiate a temporary XmlRpcServer or delegate to a shared instance.
        _xmlRpcServer = new XmlRpcServer("http://localhost:5000/xmlrpc/", robotLibrary, logger);
    }

    [HttpPost]
    public async Task<IActionResult> HandleXmlRpc()
    {
        string xmlRequest;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            xmlRequest = await reader.ReadToEndAsync();
        }

        _logger.LogInformation("Received XML-RPC request: {xml}", xmlRequest);

        // Process the XML-RPC request using the shared code.
        // In a real scenario, you might refactor the XML-RPC processing into a service method.
        string responseXml = "<xml>Placeholder response from ASP.NET service</xml>";
        return Content(responseXml, "text/xml");
    }
}