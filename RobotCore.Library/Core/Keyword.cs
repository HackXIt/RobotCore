using LoxSmoke.DocXml;
using RobotCore.Library.Attributes;
using System.Reflection;
using System.Text;

namespace RobotCore.Library.Core;
/// <summary>
/// A keyword is a method that can be called from Robot Framework.
/// This class contains all information about a C# method that qualifies as a keyword.
/// </summary>
public class Keyword
{
    public string FriendlyName { get; private set; }
    
    public object ClassInstance { get; }

    public Type ClassType => ClassInstance.GetType();
    
    public KeywordMetadata Metadata { get; private set; }
    
    public MethodComments? KeywordDocumentation { get; }

    public string KeywordDocumentationString => IsDocumentationComplete
            ? _keywordDocumentationString
            : GenerateKeywordDocumentation();
    public string[] KeywordTags { get; private set; }

    public bool HasDocumentationFile { get; }

    public bool IsDocumentationComplete { get; set; }

    private string _keywordDocumentationString = string.Empty;

    /// <summary>
    /// ctor of Keyword
    /// </summary>
    public Keyword(object classInstance, MethodInfo method, DocXmlReader? documentation)
    {
        if (method == null) throw new Exception("No keyword method specified");
        Metadata = new KeywordMetadata(method);
        //record properties
        KeywordDocumentation = null;
        KeywordTags = method.GetCustomAttributes(false).OfType<IRobotTags>().FirstOrDefault()?.Tags ?? [];
        FriendlyName = Metadata.Name.Replace("_", " ").ToUpper();
        ClassInstance = classInstance ?? throw new Exception("No class instance specified");
        // get xml documentation
        if (documentation != null)
        {
            KeywordDocumentation = documentation.GetMethodComments(method);
            HasDocumentationFile = true;
        }
        else
        {
            HasDocumentationFile = false;
            _keywordDocumentationString =
                method.GetCustomAttributes(false).OfType<IRobotDocumentation>().FirstOrDefault()?.Documentation ??
                string.Empty;
            IsDocumentationComplete = string.IsNullOrEmpty(_keywordDocumentationString);
        }
    }

    private string GenerateKeywordDocumentation()
    {
        var doc = new StringBuilder();
        if (HasDocumentationFile && KeywordDocumentation != null)
        {
            if (!string.IsNullOrEmpty(KeywordDocumentation.Summary))
            {
                doc.AppendLine($"Summary: {KeywordDocumentation.Summary}");
            }

            // Process Remarks
            if (!string.IsNullOrEmpty(KeywordDocumentation.Remarks))
            {
                doc.AppendLine($"\nRemarks: {KeywordDocumentation.Remarks}");
            }

            // Process TypeParameters
            var typeParamString = KeywordDocumentation.TypeParameters.Aggregate("", (current, typeParam)
                => current + $"{typeParam.Name}: {typeParam.Text}\n");
            if (!string.IsNullOrEmpty(typeParamString))
            {
                doc.AppendLine($"\nType Parameters: {typeParamString}");
            }

            doc.AppendLine($"\nParameter information:");
            foreach (var arg in Metadata.Arguments)
            {
                var param = KeywordDocumentation.Parameters.FirstOrDefault(p
                    => string.Compare(p.Name, arg.Key, StringComparison.OrdinalIgnoreCase) == 0);
                if (param.Text == null) continue;
                var argString = arg.Value.IsOptional
                    ? $"{arg.Key}={arg.Value.defaultValue}: {arg.Value.type.Name}"
                    : $"{arg.Key}: {arg.Value.type.Name}";
                argString += $"\n\t\t{param.Text}";
                doc.AppendLine($"\t{argString}");
            }
        }
        var returnString = HasDocumentationFile
            ? $"{Metadata.Method.ReturnType.Name}\n\t{KeywordDocumentation?.Returns ?? string.Empty}"
            : Metadata.Method.ReturnType.Name;
        doc.AppendLine($"\nReturns: {returnString}");
        _keywordDocumentationString = doc.ToString();
        IsDocumentationComplete = true;
        return _keywordDocumentationString;
    }
}