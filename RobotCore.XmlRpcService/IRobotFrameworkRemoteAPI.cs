using Horizon.XmlRpc.Core;

namespace RobotCore.XmlRpcService;

public interface IRobotFrameworkRemoteApi
{
    #region RobotFramework

    #region RF_GENERAL
    
    /// <summary>
    /// Execute a keyword with the given parameters
    /// </summary>
    /// <param name="parameters">Parameters for the keyword</param>
    /// <remarks>
    /// Due to RobotFramework's keyword execution model of either calling with name + args or name + args + kwargs,
    /// and Horizon.XmlRpc.Core's inability to handle duplicate methods (i.e. run_keyword(string, object[], XmlRpcStruct) and run_keyword(string, object[])),
    /// we have to use this method as a catch-all for the first case.
    /// </remarks>
    /// <returns>XmlRpcStruct with contents conforming to RobotFramework structure</returns>
    [XmlRpcMethod]
    public XmlRpcStruct run_keyword(params object[] parameters);

    #endregion
        
    #region RF_PRE_4.0

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Deprecated remote-library feature, necessary for backwards compatibility</remarks>
    /// <returns></returns>
    [XmlRpcMethod]
    string[] get_keyword_names();
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendlyName">The keyword to return arguments from</param>
    /// <remarks>Deprecated remote-library feature, necessary for backwards compatibility</remarks>
    /// <returns></returns>
    [XmlRpcMethod]
    string[] get_keyword_arguments(string friendlyName);
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendlyName">The keyword to return argument types from</param>
    /// <remarks>Deprecated remote-library feature, necessary for backwards compatibility</remarks>
    /// <returns></returns>
    [XmlRpcMethod]
    string[] get_keyword_types(string friendlyName);
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendlyName">The keyword to return tags from</param>
    /// <remarks>Deprecated remote-library feature, necessary for backwards compatibility</remarks>
    /// <returns></returns>
    [XmlRpcMethod]
    string[] get_keyword_tags(string friendlyName);
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendlyName">The keyword to return documentation from</param>
    /// <remarks>Deprecated remote-library feature, necessary for backwards compatibility</remarks>
    /// <returns></returns>
    [XmlRpcMethod]
    string get_keyword_documentation(string friendlyName);

    #endregion

    #region RF_SINCE_4.0

    /// <summary>
    /// Method for receiving complete library information.
    /// When this method exists, all other getters regarding keywords are not used
    /// </summary>
    /// <remarks>Feature started since Robot Framework 4.0</remarks>
    /// <returns>Information on the library, such as keywords, types, tags, documentation, etc.</returns>
    [XmlRpcMethod]
    XmlRpcStruct get_library_information();

    #endregion

    #endregion
}