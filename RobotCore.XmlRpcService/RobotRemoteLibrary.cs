using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using Horizon.XmlRpc.Core;
using Horizon.XmlRpc.Server;
using Microsoft.Extensions.Logging;
using RobotCore.Library.Attributes;
using RobotCore.Library.Core;
using RobotCore.Library.Exceptions;

namespace RobotCore.XmlRpcService;

public class RobotRemoteLibrary : XmlRpcListenerService, IRobotFrameworkRemoteApi, IXmlRpcRequestHandler
{
    /// <summary>
    /// NLog instance
    /// </summary>
    private readonly ILogger<RobotRemoteLibrary> _logger;

    #region Properties

    private readonly IKeywordManager _keywordManager;
    private readonly ConcurrentDictionary<int, string> _threadKeywordType = new();

    #endregion

    #region Ctor

    public RobotRemoteLibrary(ILogger<RobotRemoteLibrary> logger, IKeywordManager keywordManager)
    {
        _logger = logger;
        _keywordManager = keywordManager;
    }

    #endregion

    #region XmlRpcListenerService

    /// <summary>
    /// Process XML-RPC request
    /// </summary>
    /// <param name="requestContext"></param>
    /// <exception cref="SystemException">Throws exception when concurrency problem occurs (thread already processing request)</exception>
    public override void ProcessRequest(HttpListenerContext requestContext)
    {
        //record url of request into property
        var id = Thread.CurrentThread.ManagedThreadId;
        // suppressing since this should always have context provided by HttpService
        var seg = requestContext.Request.Url!.Segments;
        var typeUrl = string.Join("", seg, 1, seg.Length - 1).Replace("/", ".");
        if (!_threadKeywordType.TryAdd(id, typeUrl))
        {
            throw new SystemException($"Thread with ID {id} is already processing a request");
        }
        //process request
        base.ProcessRequest(requestContext);
        //remove thread property
        if (!_threadKeywordType.TryRemove(id, out typeUrl))
        {
            throw new SystemException($"Failed to remove Thread with ID {id} from thread property");
        }
    }

    #endregion

    #region RobotFramework
        
    #region RF_General

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
    public XmlRpcStruct run_keyword(params object[] parameters)
    {
        // Check that at least two parameters are provided.
        if (parameters.Length < 2)
        {
            throw new XmlRpcFaultException(1, "Insufficient parameters provided to run_keyword.");
        }

        // Extract the keyword name.
        if (parameters[0] is not string friendlyName)
        {
            throw new XmlRpcFaultException(1, "First parameter must be a string (keyword name).");
        }

        // Extract the positional arguments.
        // Accept any array type (e.g. int[], string[], etc.) and convert to object[].
        object[] positionalArgs;
        if (parameters[1] is Array posArray)
        {
            positionalArgs = posArray.Cast<object>().ToArray();
        }
        else
        {
            throw new XmlRpcFaultException(1, "Second parameter must be an array (positional arguments).");
        }

        // Extract named arguments if provided.
        XmlRpcStruct? kwargs = null;
        if (parameters.Length < 3) return RunKeywordInternal(friendlyName, positionalArgs, kwargs);
        if (parameters[2] is XmlRpcStruct structArg)
        {
            kwargs = structArg;
        }
        else
        {
            throw new XmlRpcFaultException(1, "Third parameter must be a struct (named arguments).");
        }

        // Dispatch to your internal implementation that merges positional and named args.
        return RunKeywordInternal(friendlyName, positionalArgs, kwargs);
    }
    
    /// <summary>
    /// Run specified Robot Framework keyword (with named arguments)
    /// </summary>
    /// <param name="friendlyName">FriendlyName of keyword to run</param>
    /// <param name="args">Arguments for calling keyword</param>
    /// <param name="kwargs">Named argument list for calling keyword</param>
    /// <returns>XmlRpcStruct with contents conforming to RobotFramework structure</returns>
    /// <exception cref="XmlRpcFaultException">Throws application error, if any Exception occurs outside of the TargetInvocation.</exception>
    private XmlRpcStruct RunKeywordInternal(string friendlyName, object[] args, XmlRpcStruct? kwargs)
    {
        _logger.LogDebug("XmlRpc method call - run_keyword: {friendlyName} with args [{args}]", friendlyName, string.Join(",", args));
        XmlRpcStruct xmlRpc;
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            var keyword = _keywordManager.GetKeyword(typename, friendlyName);
            KeywordResult result;
            // Pre-process arguments for conversion of arrays to lists/dictionaries (SupportedTypes)
            args = PreProcessArgumentTypes(keyword.Metadata.Method, args);
            result = kwargs == null
                ? _keywordManager.RunKeyword(keyword, args)
                : _keywordManager.RunKeyword(keyword, args, ProcessNamedArguments(kwargs));
            result.KeywordReturn = PostProcessReturnType(keyword.Metadata.Method, result.KeywordReturn);
            _logger.LogDebug(result.ToString());
            xmlRpc = XmlRpcBuilder.FromKeywordResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - run_keyword {friendlyName}: {message}\n{stackTrace}", friendlyName, e.Message, e.StackTrace);
            throw new XmlRpcFaultException(1, e.Message);
        }
        return xmlRpc;
    }

    #endregion // RF_General

    #region RF_PRE_4.0

    /// <summary>
    /// Get a list of keywords available for use
    /// </summary>
    /// <returns>A list of strings (available keywords in the library)</returns>
    /// <exception cref="XmlRpcFaultException">Throws application error, when KeywordLoadingException occurs</exception>
    public string[] get_keyword_names()
    {
        try 
        {
            _logger.LogDebug("XmlRpc Method call - get_keyword_names");
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            return _keywordManager.GetKeywordNames(typename);
        }
        catch (LibraryException.LoadingException e)
        {
            _logger.LogError("Exception in method - get_keyword_names : {message}", e.Message);
            throw new XmlRpcFaultException(1,e.Message);
        }
    }

    /// <summary>
    /// Get a list of arguments for the given keyword
    /// </summary>
    /// <param name="friendlyName">FriendlyName of keyword to search</param>
    /// <returns>A list of strings (arguments of the keyword)</returns>
    public string[] get_keyword_arguments(string friendlyName)
    {
        _logger.LogDebug($"XmlRpc Method call - get_keyword_arguments {friendlyName}");
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            return _keywordManager.GetArgumentNamesForKeyword(typename, friendlyName).ToArray();
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - get_keyword_arguments : {message}", e.Message);
            throw new XmlRpcFaultException(1,e.Message);
        }
    }

    public string[] get_keyword_types(string friendlyName)
    {
        _logger.LogDebug("XmlRpc Method call - get_keyword_types {friendlyName}", friendlyName);
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            return _keywordManager.GetArgumentTypesForKeyword(typename, friendlyName);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - get_keyword_types : {message}", e.Message);
            throw new XmlRpcFaultException(1,e.Message);
        }
    }

    public string[] get_keyword_tags(string friendlyName)
    {
        _logger.LogDebug("XmlRpc Method call - get_keyword_tags {friendlyName}", friendlyName);
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            return _keywordManager.GetKeyword(typename, friendlyName).KeywordTags;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - get_keyword_tags : {message}", e.Message);
            throw new XmlRpcFaultException(1, e.Message);
        }
    }

    /// <summary>
    /// Get documentation for specified Robot Framework keyword.
    /// Done by reading the .NET compiler generated XML documentation
    /// for the loaded class library.
    /// </summary>
    /// <returns>A documentation string for the given keyword.</returns>
    /// <remarks>When use</remarks>
    public string get_keyword_documentation(string friendlyName)
    {
        _logger.LogDebug("XmlRpc Method call - get_keyword_documentation {friendlyName}", friendlyName);
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            var keyword = _keywordManager.GetKeyword(typename, friendlyName);
            //check for INTRO 
            if (string.Equals(friendlyName, KeywordManager.CIntro, StringComparison.CurrentCultureIgnoreCase))
            {
                // TODO Implement keyword documentation __INTRO__
                return keyword.ClassType.GetCustomAttribute<RobotLibraryAttribute>()?.Introduction ?? string.Empty;
            }
            //check for init
            if (string.Equals(friendlyName, KeywordManager.CInit, StringComparison.CurrentCultureIgnoreCase))
            {
                // TODO Implement keyword documentation __INIT__
                return keyword.ClassType.GetCustomAttribute<RobotLibraryAttribute>()?.Initialization ?? string.Empty;
            }
            //get keyword documentation
            return keyword.KeywordDocumentationString;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - get_keyword_documentation : {message}", e.Message);
            throw new XmlRpcFaultException(1,e.Message);
        }
    }

    #endregion // RF_PRE_4.0

    #region RF_SINCE_4.0

    public XmlRpcStruct get_library_information()
    {
        _logger.LogDebug("XmlRpc Method call - get_library_information");
        try
        {
            var typename = _threadKeywordType[Thread.CurrentThread.ManagedThreadId];
            return XmlRpcBuilder.FromLibraryObject(_keywordManager.GetLibraryInformation(typename)); 
        }
        catch (Exception e)
        {
            _logger.LogError("Exception in method - get_library_information : {message}", e.Message);
            throw new XmlRpcFaultException(1,e.Message);
        }
    }

    #endregion // RF_SINCE_4.0
    
    #endregion // RobotFramework

    #region PrivateMethods

    private Dictionary<string, object> ProcessNamedArguments(XmlRpcStruct namedArguments)
    {
        var namedArgs = new Dictionary<string, object>();
        var index = 0;
        foreach (DictionaryEntry entry in namedArguments)
        {
            if (entry.Key == null)
                throw new RobotException.FatalKeywordException($"Named argument entry [{index}] is null. Cannot convert to {namedArgs.GetType()}.");
            if (!(entry.Key is string))
                throw new RobotException.FatalKeywordException($"Named argument [{index}].Key is not a string. Cannot convert to {namedArgs.GetType()}.");
            if (entry.Value is null)
                throw new RobotException.FatalKeywordException($"Named argument [{index}].Value is null. Cannot convert to {namedArgs.GetType()}.");
            namedArgs.Add((string)entry.Key, entry.Value);
            _logger.LogDebug("Added named argument entry[{index}]: {key}={value}", index, entry.Key, entry.Value);
            index++;
        }
        return namedArgs;
    }

    private object[] PreProcessArgumentTypes(MethodInfo method, object arguments)
    {
        object[] args;
        if (arguments is Array argArray)
        {
            args = argArray.OfType<object>().ToArray();
        }
        else
        {
            // Edge cases not handled still get encapsulated as array inside of array
            // This will surely produce an error, but not for our supported types
            args = new[] { arguments };
        }
        // Pre-process arguments to convert arrays to lists
        var methodParameters = method.GetParameters();
        var preProcessedArgs = new object[args.Length];

        for (var i = 0; i < args.Length; i++)
        {
            switch (methodParameters[i].ParameterType.IsGenericType)
            {
                case true when methodParameters[i].ParameterType.GetGenericTypeDefinition() == typeof(List<>):
                    var listType = methodParameters[i].ParameterType.GenericTypeArguments[0];
                    preProcessedArgs[i] = ConvertToList(listType, args[i]);
                    break;
                case true when methodParameters[i].ParameterType.GetGenericTypeDefinition() == typeof(Dictionary<,>):
                    var keyType = methodParameters[i].ParameterType.GenericTypeArguments[0];
                    var valueType = methodParameters[i].ParameterType.GenericTypeArguments[1];
                    preProcessedArgs[i] = ConvertToDictionary(keyType, valueType, args[i]);
                    break;
                default:
                    _logger.LogDebug("No conversion for argument {index} of type {parameterType}", i, methodParameters[i].ParameterType);
                    preProcessedArgs[i] = args[i];
                    break;
            }
        }
        return preProcessedArgs;
    }

    private object? PostProcessReturnType(MethodInfo method, object? returnValue)
    {
        if (returnValue == null)
            return null;
        var returnType = method.ReturnType;
        return returnType.IsGenericType switch
        {
            true when returnType.GetGenericTypeDefinition() == typeof(List<>) 
                => ConvertFromList(returnValue),
            true when returnType.GetGenericTypeDefinition() == typeof(Dictionary<,>) 
                => ConvertFromDictionary(returnValue),
            _ => returnValue
        };
    }
    
    private object ConvertToList(Type elementType, object toConvert)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = Activator.CreateInstance(listType);
        if (list == null)
            throw new RobotException.FatalKeywordException($"Unable to create instance of type {listType}.");
        var addMethod = listType.GetMethod("Add");
        if (addMethod == null)
            throw new RobotException.FatalKeywordException($"Unable to find Add method in type {listType}.");

        if (toConvert is not IEnumerable enumerable) return list;
        foreach (var item in enumerable)
        {
            var element = item switch
            {
                XmlRpcStruct =>
                    // Assuming XmlRpcStruct to Dictionary<string, object> conversion
                    ConvertToDictionary(typeof(string), elementType.GenericTypeArguments[1], item),
                IEnumerable when elementType.IsGenericType =>
                    // Recursively convert nested lists
                    ConvertToList(elementType.GenericTypeArguments[0], item),
                _ => item
            };
            addMethod.Invoke(list, [element]);
        }
        return list;
    }
    
    private object ConvertToDictionary(Type keyType, Type valueType, object toConvert)
    {
        var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dictionary = Activator.CreateInstance(dictType);
        if (dictionary == null)
            throw new RobotException.FatalKeywordException($"Unable to create instance of type {dictType}.");
        var addMethod = dictType.GetMethod("Add");
        if (addMethod == null)
            throw new RobotException.FatalKeywordException($"Unable to find Add method in type {dictType}.");

        if (toConvert is not XmlRpcStruct xmlRpcStruct) return dictionary;
        foreach (DictionaryEntry entry in xmlRpcStruct)
        {
            if (entry.Value == null)
                throw new RobotException.FatalKeywordException($"Value of entry {entry.Key} is null. Cannot convert to {valueType}.");
            var value = entry.Value switch
            {
                XmlRpcStruct =>
                    // Assuming XmlRpcStruct to Dictionary<string, object> conversion
                    ConvertToDictionary(typeof(string), valueType.GenericTypeArguments[1], entry.Value),
                IEnumerable when valueType.IsGenericType =>
                    // Recursively convert nested lists
                    ConvertToList(valueType.GenericTypeArguments[0], entry.Value),
                _ => entry.Value
            };
            addMethod.Invoke(dictionary, new[] { entry.Key, value });
        }
        return dictionary;
    }

    private object?[] ConvertFromList(object toConvert)
    {
        if (toConvert is IList list)
        {
            var arrayList = new ArrayList();
            foreach (var item in list)
            {
                if (item is IDictionary dictItem)
                {
                    var xmlRpcStruct = ConvertFromDictionary(dictItem);
                    arrayList.Add(xmlRpcStruct);
                }
                else
                {
                    arrayList.Add(item); // Simple types are directly added
                }
            }
            return arrayList.ToArray(); // Convert to array as XMLRPC typically uses arrays
        }
        throw new RobotException.FatalKeywordException($"Unable to convert object={toConvert} to XMLRPC array type.");
    }

    private XmlRpcStruct ConvertFromDictionary(object toConvert)
    {
        if (toConvert is IDictionary dict)
        {
            var xmlRpcStruct = new XmlRpcStruct();
            foreach (DictionaryEntry entry in dict)
            {
                var key = entry.Key.ToString();
                if (key == null)
                    throw new RobotException.FatalKeywordException(
                        $"Key of entry {entry} is null. Cannot convert to string.");
                var value = entry.Value;

                if (value is IList listValue)
                {
                    var xmlRpcArray = ConvertFromList(listValue);
                    xmlRpcStruct.Add(key, xmlRpcArray);
                }
                else
                {
                    xmlRpcStruct.Add(key, value); // Simple types are directly added
                }
            }

            return xmlRpcStruct;
        }

        throw new RobotException.FatalKeywordException(
            $"Unable to convert object={toConvert} to XMLRPC struct type.");
    }

    #endregion // PrivateMethods
}