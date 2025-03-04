
namespace RobotCore.Library.Core;

public interface IKeywordManager
{
    #region LibraryManagement

    void AddLibrary(string assemblyPath, string typename);
    
    void AddLibrary(Type libraryType);
    
    void AddLibrary(object libraryInstance);

    #endregion

    #region LibraryManagement

    #region RF_General

    KeywordResult RunKeyword(Keyword keyword, object[] arguments, Dictionary<string, object> namedArguments = null);

    #endregion RF_General

    #region RF_PRE_4.0

    Keyword GetKeyword(string typename, string friendlyName);

    string[] GetKeywordNames(string typename);

    List<string> GetArgumentNamesForKeyword(string typename, string friendlyName);

    string[] GetArgumentTypesForKeyword(string typename, string friendlyName);

    string[] GetLoadedLibraryNames();

    #endregion RF_PRE_4.0

    #region RF_SINCE_4.0

    Dictionary<string, Dictionary<string, object>> GetLibraryInformation(string typename);

    #endregion RF_SINCE_4.0

    #endregion RobotFramework

}