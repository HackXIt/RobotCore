using RobotCore.Library.Exceptions;
using RobotCore.Library.Utility;

namespace RobotCore.Library.Core;

/// <summary>
/// A formatted result of a keyword execution suitable for Robot Framework
/// </summary>
public class KeywordResult
{
    #region NestedTypes

    public enum KeywordStatus
    {
        [StringValue("PASS")] Pass,
        [StringValue("FAIL")] Fail
    }

    #endregion
    
    #region GenericProperties

    public KeywordStatus Status { get; set; }
        
    public RobotException.RobotExceptionType? ErrorType { get; set; }

    #endregion

    #region RunKeywordProperties

    public string KeywordOutput { get; set; }
    public object? KeywordReturn { get; set; }
    public string KeywordError { get; set; }
    public string KeywordTraceback { get; set; }
    public double KeywordDuration { get; set; }

    #endregion

    public override string ToString()
    {
        var keywordReturn = "Null";
        if (KeywordReturn != null)
        {
            //check if array return type
            if (KeywordReturn.GetType().IsArray && KeywordReturn.GetType().GetElementType() == typeof(string))
            {
                keywordReturn = "[" + string.Join(",", (string[])KeywordReturn) + "]";
            }
            else
            {
                keywordReturn = KeywordReturn.ToString();
            }
        }

        return "[CommandResult " + 
               $"Status={Status}, " + 
               $"Output={KeywordOutput}, " + 
               $"Return={keywordReturn}, " + 
               $"Error={KeywordError}, " + 
               $"Traceback={KeywordTraceback}, " + 
               $"Duration={KeywordDuration}, " + 
               $"ErrorType={ErrorType}]";
    }

    public void CaptureException(Exception ex)
    {
        KeywordTraceback = ex.StackTrace ?? "No stack trace available";
        KeywordError = ex.Message;
        Status = KeywordStatus.Fail;
        KeywordReturn = null; // NOTE In case of error, return value is always null (i.e. 'None' in RF)
            
        // Check exception type and set error type accordingly
        ErrorType = ex switch
        {
            RobotException.ContinuableKeywordException => RobotException.RobotExceptionType.Continuable,
            RobotException.FatalKeywordException => RobotException.RobotExceptionType.Fatal,
            _ => RobotException.RobotExceptionType.Fail
        };
    }
}