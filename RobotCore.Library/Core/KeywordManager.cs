using LoxSmoke.DocXml;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using RobotCore.Library.Attributes;
using RobotCore.Library.Exceptions;
using RobotCore.Library.Utility;

namespace RobotCore.Library.Core;

/// <summary>
/// A manager for keyword libraries
/// </summary>
public class KeywordManager : IKeywordManager
{
    // TODO Add ToString for better class representation in Logs

    #region StaticStrings

    public const string CIntro = "__intro__";
    public const string CInit = "__init__";
    public const string CKeywordArguments = "args";
    public const string CKeywordTypes = "types";
    public const string CKeywordDocumentation = "doc";
    public const string CKeywordTags = "tags";

    #endregion StaticStrings

    /// <summary>
    /// Logging instance
    /// </summary>
    private readonly ILogger<KeywordManager> _logger;

    private readonly Dictionary<string, List<Keyword>> _loadedKeywords = new();
    private readonly object _lock = new();

    /// <summary>
    /// Constructor with default supported types
    /// </summary>
    public KeywordManager(ILogger<KeywordManager> logger)
    {
        _logger = logger;
    }

    #region LibraryManagement

    #region IKeywordManager

    /// <summary>
    /// Adds a library via assembly loading to the manager
    /// </summary>
    /// <param name="assemblyPath">Absolute path to the assembly</param>
    /// <param name="typename">Type name of the library to add</param>
    public void AddLibrary(string assemblyPath, string typename)
    {
        if (string.IsNullOrEmpty(assemblyPath))
            throw new ArgumentNullException(nameof(assemblyPath));
        if (string.IsNullOrEmpty(typename))
            throw new ArgumentNullException(nameof(typename));
        if (!File.Exists(assemblyPath))
            throw new ArgumentException($"Assembly '{assemblyPath}' does not exist.", nameof(assemblyPath));

        _logger.LogDebug("Loading type {typename}", typename);
        
        // Load the assembly
        var assembly = Assembly.LoadFrom(assemblyPath);
        // Use the type's full name as the key
        var libraryType = assembly.GetType(typename)
                                 ?? throw new ArgumentException($"Type '{typename}' not found in assembly '{assemblyPath}'", nameof(typename));
        // Create an instance using Activator.CreateInstance and any additional reflection as needed
        var libraryInstance = Activator.CreateInstance(libraryType)
                              ?? throw new LibraryException($"Could not create instance of {typename}");
        
        AddLibrary(libraryInstance);
    }

    public void AddLibrary(Type libraryType)
    {
        // Create an instance from the type.
        var libraryInstance = Activator.CreateInstance(libraryType)
                              ?? throw new LibraryException($"Failed to create instance of {libraryType.FullName}");

        AddLibrary(libraryInstance);
    }
    
    /// <summary>
    /// Adds a library instance to the manager.
    /// </summary>
    /// <param name="libraryInstance">The instance of the library.</param>
    /// <param name="docXmlPath">Optional path to the documentation file for the library/assembly.</param>
    /// <exception cref="ArgumentException">Thrown when the library instance does not have a valid type.</exception>
    public void AddLibrary(object libraryInstance)
    {
        var libraryType = libraryInstance.GetType();
        var typename = libraryType.Name;
        if (string.IsNullOrEmpty(typename))
            throw new ArgumentException("Library instance must have a valid typename", nameof(libraryInstance));
        
        // Optionally load the XML documentation file from the assembly's directory (if available)
        DocXmlReader? docReader = null;
        var assembly = libraryType.Assembly;
        if (!string.IsNullOrEmpty(assembly.Location))
        {
            // Change the extension to .xml (e.g., MyLibrary.dll -> MyLibrary.xml)
            var xmlPath = Path.ChangeExtension(assembly.Location, ".xml");
            if (File.Exists(xmlPath))
            {
                docReader = new DocXmlReader(xmlPath);
            }
        }

        lock (_lock)
        {
            if (_loadedKeywords.ContainsKey(typename))
            {
                _logger.LogDebug("Type {typename} is already loaded", typename);
                return;
            }

            var keywords = DiscoverKeywords(libraryInstance, libraryType, docReader);
            _loadedKeywords.Add(typename, keywords);
            _logger.LogDebug("Loaded keywords: {keywords}", string.Join(",", keywords.Select(k => k.FriendlyName)));
        }
    }
    
    #endregion // IKeywordManager
    
    private List<Keyword> DiscoverKeywords(object instance, Type libraryType, DocXmlReader? docXml = null)
    {
        // Use reflection to find public methods (excluding those from Object)
        IEnumerable<MethodInfo> methods;
        if (libraryType.GetCustomAttribute<RobotLibraryAttribute>()?.ExplicitDiscovery == true)
        {
            methods = libraryType.GetMethods().Where(mi => mi.DeclaringType != typeof(object)
                && mi.GetCustomAttribute<RobotKeywordAttribute>() != null);
        }
        else
        {
            methods = libraryType.GetMethods().Where(mi => mi.DeclaringType != typeof(object));
            return (from method in methods where HasValidSignature(method) 
                select new Keyword(instance, method, docXml)).ToList();
        }
        return (from method in methods where HasValidSignature(method) 
            select new Keyword(instance, method, docXml)).ToList();
    }

    /// <summary>
    /// Checks whether a method’s signature is valid for a keyword.
    /// </summary>
    private bool HasValidSignature(MethodInfo methodInfo)
    {
        // Verify that the return type is supported.
        if (!RobotTypeConverter.IsValidType(methodInfo.ReturnParameter.ParameterType))
            return false;
        // The method must be public.
        if (!methodInfo.IsPublic)
            return false;
        // Allow obsolete methods.
        // TODO Should become configurable
        var customAttributes = methodInfo.GetCustomAttributes(false);
        if (customAttributes.Any(attribute => attribute is ObsoleteAttribute))
            return true;
        // All parameter types must be supported.
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (methodInfo.GetParameters().Any(parameter => !RobotTypeConverter.IsValidType(parameter.ParameterType)))
            return false;
        
        // All checks passed
        return true;
    }

    #endregion LibraryManagement

    #region RF_General

    /// <summary>
    /// Attempts to invoke the provided keyword with provided arguments
    /// </summary>
    /// <param name="keyword">A given keyword from the loaded libraries</param>
    /// <param name="arguments">A list of arguments of the keyword, if any</param>
    /// <param name="namedArgs">A dictionary of named arguments of the keyword, if any</param>
    /// <returns>
    /// Result object conforming to RobotFramework library result in dictionary format.
    /// </returns>
    /// <exception cref="RobotException.FatalKeywordException">
    /// Throws exception, if provided keyword or arguments are faulty.
    /// </exception>
    public KeywordResult RunKeyword(Keyword keyword, object[] positionalArgs, Dictionary<string, object>? namedArgs = null)
    {
        // Setup execution variables
        var result = new KeywordResult();
        var timer = new Stopwatch();
        var traceContent = new MemoryStream();
        var utf8IgnoreIllegalCharacters = new SanitizedXmlEncoding();
        var traceWriter = new StreamWriter(traceContent, utf8IgnoreIllegalCharacters);
        var traceListener = new TextWriterTraceListener(traceWriter);
        Trace.Listeners.Add(traceListener);

        try
        {
            // Shortcut variables
            var method = keyword.Metadata.Method;
            var totalParameters = keyword.Metadata.ArgumentNames.Count;
            
            // Create the final arguments array with a fixed size
            var finalArguments = new object[totalParameters];
            // Boolean flags to track which arguments have been explicitly provided.
            var providedFlags = new bool[totalParameters];
            
            // 1. Fill finalArguments with positional arguments (if any)
            for (var i = 0; i < positionalArgs.Length && i < totalParameters; i++)
            {
                finalArguments[i] = positionalArgs[i];
                providedFlags[i] = true;
            }
            
            // 2. Override with named arguments if provided.
            // This works even if the positional array was shorter than expected.
            if (namedArgs != null)
            {
                foreach (var kvp in namedArgs)
                {
                    var argumentIndex = keyword.Metadata.ArgumentNames.IndexOf(kvp.Key);
                    if (argumentIndex == -1)
                    {
                        throw new RobotException.FatalKeywordException(
                            $"Unexpected named argument '{kvp.Key}' for {keyword.FriendlyName}. Expected one of: {string.Join(", ", keyword.Metadata.ArgumentNames)}");
                    }

                    finalArguments[argumentIndex] = kvp.Value;
                    providedFlags[argumentIndex] = true;
                }
            }
            
            // 3. For optional parameters not provided, apply their default values.
            if (keyword.Metadata.HasOptionalParameters)
            {
                foreach (var optionalArg in keyword.Metadata.OptionalArguments)
                {
                    var index = keyword.Metadata.ArgumentNames.IndexOf(optionalArg.Key);
                    // Only apply default if the argument was not explicitly provided.
                    if (!providedFlags[index])
                    {
                        finalArguments[index] = optionalArg.Value.obj;
                    }
                }
            }
            
            // 4. Validate that all required arguments are present.
            foreach (var requiredArg in 
                     from requiredArg in keyword.Metadata.RequiredArguments 
                     let index = keyword.Metadata.ArgumentNames.IndexOf(requiredArg.Key) 
                     where finalArguments[index] == null 
                     select requiredArg)
            {
                throw new RobotException.FatalKeywordException(
                    $"Missing required argument '{requiredArg.Key}' for {keyword.FriendlyName}.");
            }

            // Write invocation string to better see in logs how the actual method call looked like
            var invocationString = $"{keyword.Metadata.Method.Name}" +
                                   "(" +
                                   string.Join(", ", positionalArgs.Select((arg, i)
                                       => $"{keyword.Metadata.ArgumentNames[i]}={GetArgumentAsString(arg)}"))
                                   + ")";

            // Example output: "Keyword invocation: String_ParameterType(arg1=one, arg2=two) ..."
            _logger.LogDebug($"Keyword invocation: {invocationString} ...");

            result.KeywordReturn = null;
            // Execute keyword method
            timer.Start();
            if (method.ReturnParameter.ParameterType == typeof(void))
            {
                method.Invoke(keyword.ClassInstance, finalArguments);
            }
            else
            {
                result.KeywordReturn = method.Invoke(keyword.ClassInstance, finalArguments);
            }
            timer.Stop();

            // Execution was successful
            result.Status = KeywordResult.KeywordStatus.Pass;
            result.ErrorType = null;
        }
        catch (TargetInvocationException te)
        {
            result.CaptureException(te.InnerException ?? te);
        }
        catch (Exception e)
        {
            result.CaptureException(e);
        }
        finally
        {
            // Record keyword execution time
            if (timer.IsRunning)
                timer.Stop();
            result.KeywordDuration = timer.Elapsed.TotalSeconds;

            // Store keyword trace output
            traceListener.Flush();
            Trace.Listeners.Remove(traceListener);
            result.KeywordOutput = utf8IgnoreIllegalCharacters.GetString(traceContent.ToArray());

            // Clean up
            traceContent.SetLength(0);
            traceListener.Dispose();
            traceContent.Dispose();
        }
        return result;
    }

    #endregion RF_General

    #region RF_PRE_4.0

    /// <summary>
    /// Returns keyword with given method name from provided library-type 
    /// </summary>
    /// <param name="typename">Class type name of keyword library to search</param>
    /// <param name="friendlyName">Friendly method name to search in library</param>
    /// <returns>A keyword object, representing the provided method in the keyword library</returns>
    /// <exception cref="LibraryException.LoadingException">
    /// Throws exception, if keyword library or method name cannot be found. 
    /// </exception>
    public Keyword GetKeyword(string typename, string friendlyName)
    {
        var result = IsTypenameValid(typename).Find(kw
            => string.Equals(kw.FriendlyName, friendlyName, StringComparison.CurrentCultureIgnoreCase));
        if (result == null)
            throw new LibraryException.LoadingException($"Keyword {friendlyName} not found in type {typename}");
        return result;

    }

    /// <summary>
    /// Returns friendly names of keywords from provided library-type
    /// </summary>
    /// <param name="typename">Class type name of keyword library to search</param>
    /// <returns>Array of strings listing keyword friendly names</returns>
    /// <exception cref="LibraryException.LoadingException">
    /// Throws exception, if provided typename is null/empty or if the provided library-type is not loaded
    /// </exception>
    public string[] GetKeywordNames(string typename)
        => IsTypenameValid(typename).Select(kw => kw.FriendlyName).ToArray();

    /// <summary>
    /// Returns argument names of keyword from provided library-type
    /// </summary>
    /// <param name="typename">Class type name of keyword library to search</param>
    /// <param name="friendlyName">Name of keyword to search</param>
    /// <returns>Array of strings listing argument names of keyword</returns>
    /// <exception cref="LibraryException.LoadingException">
    /// Throws exception, if provided typename is null/empty or if the provided library-type is not loaded
    /// </exception>
    public List<string> GetArgumentNamesForKeyword(string typename, string friendlyName)
        => GetKeyword(typename, friendlyName).Metadata.ArgumentNames;

    /// <summary>
    /// Returns robot framework conforming type names for arguments of keyword from provided library-type
    /// </summary>
    /// <param name="typename">Class type name of keyword library to search</param>
    /// <param name="friendlyName">Name of keyword to search</param>
    /// <returns>Array of strings listing robot framework conforming type names for arguments of keyword</returns>
    /// <exception cref="LibraryException.LoadingException">
    /// Throws exception, if provided typename is null/empty or if the provided library-type is not loaded
    /// </exception>
    public string[] GetArgumentTypesForKeyword(string typename, string friendlyName)
        => GetKeyword(typename, friendlyName).Metadata.ArgumentTypes.Select(RobotTypeConverter.ConvertTypeToRobotFramework).ToArray();

    /// <summary>
    /// Returns keyword library class type names
    /// </summary>
    /// <returns>Array of strings listing loaded keyword library class type names</returns>
    public string[] GetLoadedLibraryNames() => _loadedKeywords.Keys.ToArray();

    #endregion RF_PRE_4.0

    #region RF_SINCE_4.0

    /// <summary>
    /// Provides full information on all loaded keyword libraries and keywords in one call
    /// </summary>
    /// <returns>An array of strings representing a dictionary which contains library information conforming to RobotFramework.</returns>
    public Dictionary<string, Dictionary<string, object>> GetLibraryInformation(string typename)
        => GetAllKeywordInformationForType(typename);

    #endregion RF_SINCE_4.0

    #region AssemblyResolver


    /// <summary>
    /// Handles AssemblyResolve event
    /// This is needed if keyword assembly has dependencies
    /// We attempt to load assembly from same directory as the keyword assembly
    /// </summary>
    public static Assembly? KeywordAssemblyResolveHandler(object source, ResolveEventArgs e)
    {
        try
        {
            // Check if library specified includes a path
            if (!e.Name.Contains("\\")) return null;
            // Check if library path exists
            var libPath = Path.GetDirectoryName(e.Name);
            if (string.IsNullOrEmpty(libPath)) return null;
            var asmName = new AssemblyName(e.Name);
            if (string.IsNullOrEmpty(asmName.Name)) return null;
            var asmPath = Path.Combine(libPath, asmName.Name);
            return Assembly.LoadFrom(asmPath);
        }
        catch
        {
            return null;
        }

    }

    #endregion AssemblyResolver

    #region PRIVATE HELPERS

    private string GetArgumentAsString(object arg)
    {
        switch (arg.GetType().IsGenericType)
        {
            case true when arg.GetType().GetGenericTypeDefinition() == typeof(List<>):
                return string.Join(",", arg);
            case true when arg.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>):
                var dict = arg as Dictionary<string, dynamic>;
                return $"[{string.Join(",", dict?.Select(kv => $"{kv.Key}={kv.Value}") ?? Array.Empty<string>())}]";
            default:
                return arg.ToString() ?? string.Empty;
        }
    }

    private List<Keyword> IsTypenameValid(string typename)
    {
        if (string.IsNullOrEmpty(typename))
            throw new LibraryException.LoadingException("Specified typename is null or empty");
        if (!_loadedKeywords.TryGetValue(typename, out var valid))
            throw new LibraryException.LoadingException($"Type {typename} not found in loaded keywords");
        return valid;
    }

    private static string GetLibraryIntroduction(MemberInfo libraryType)
        => Attribute.GetCustomAttribute(libraryType, typeof(RobotLibraryAttribute)) is not RobotLibraryAttribute attribute
            ? string.Empty
            : attribute.Introduction;

    private static string GetLibraryInitialization(MemberInfo libraryType)
        => Attribute.GetCustomAttribute(libraryType, typeof(RobotLibraryAttribute)) is not RobotLibraryAttribute attribute
            ? string.Empty
            : attribute.Initialization;

    private Dictionary<string, Dictionary<string, object>> GetAllKeywordInformationForType(string typename)
    {
        var library = IsTypenameValid(typename);
        var libraryIntro = GetLibraryIntroduction(library[0].ClassType);
        var libraryInit = GetLibraryInitialization(library[0].ClassType);
        var result = new Dictionary<string, Dictionary<string, dynamic>>();
        result.Add(CIntro, new Dictionary<string, dynamic>());
        result[CIntro].Add("doc", libraryIntro);
        result.Add(CInit, new Dictionary<string, dynamic>());
        result[CInit].Add("doc", libraryInit);
        foreach (var keyword in library)
        {
            var kwDoc = new Dictionary<string, dynamic>();
            if (keyword.Metadata.HasOptionalParameters)
            {
                var requiredArgs = keyword.Metadata.RequiredArguments.Keys.ToArray();
                var optionalArgs = keyword.Metadata.OptionalArguments
                    .Select(a => $"{a.Key}={a.Value.obj}").ToArray();
                var combinedArgs = requiredArgs.Concat(optionalArgs).ToArray();
                kwDoc.Add(CKeywordArguments, combinedArgs);
            }
            else
            {
                kwDoc.Add(CKeywordArguments, keyword.Metadata.ArgumentNames.ToArray());
            }

            kwDoc.Add(CKeywordTypes, keyword.Metadata.ArgumentTypes.Select(RobotTypeConverter.ConvertTypeToRobotFramework).ToArray());
            kwDoc.Add(CKeywordDocumentation, keyword.KeywordDocumentationString);
            kwDoc.Add(CKeywordTags, keyword.KeywordTags);
            try
            {
                result.Add(keyword.FriendlyName, kwDoc);
            }
            catch (ArgumentException ex)
            {
                throw new LibraryException.LoadingException($"Keyword name '{keyword.FriendlyName}' exists more than once!", ex);
            }
        }

        return result;
    }

    #endregion PRIVATE HELPERS
}