using Horizon.XmlRpc.Client;

namespace RobotCore.XmlRpcService;

/// <summary>
/// Interface to define a client proxy for robot remote 
/// </summary>
/// <remarks>
/// See <see href="https://github.com/claytonneal/nrobot-server/blob/master/NRobot.Server.Imp/Services/IRemoteClient.cs">IRemoteClient</see> for old implementation from NRobot-Server.
/// </remarks>
public interface IRemoteClient : IXmlRpcProxy, IRobotFrameworkRemoteApi
{
        
}