using System.Net;

namespace RobotCore.XmlRpcService;

public interface IXmlRpcRequestHandler
{
    #region RequestHandler
        
    void ProcessRequest(HttpListenerContext requestContext);

    #endregion
}