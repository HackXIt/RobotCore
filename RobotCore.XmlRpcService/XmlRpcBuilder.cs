using Horizon.XmlRpc.Core;
using RobotCore.Library.Core;
using RobotCore.Library.Exceptions;
using RobotCore.Library.Utility;

namespace RobotCore.XmlRpcService;

/// <summary>
/// A builder class for constructing XmlRpc results conforming to RobotFramework structure
/// </summary>
public static class XmlRpcBuilder
{
    /// <summary>
    /// Builds a structured XmlRpc result from a provided command result
    /// </summary>
    /// <param name="kwResult">The result from the command which was run</param>
    /// <returns>XmlRpcStruct with contents conforming to RobotFramework structure</returns>
    public static XmlRpcStruct FromKeywordResult(KeywordResult kwResult)
    {
        var result = new XmlRpcStruct
        {
            // Add status
            { "status", kwResult.Status.GetStringValue() },
            // Add error
            { "error", string.IsNullOrEmpty(kwResult.KeywordError) ? "" : kwResult.KeywordError },
            // Add traceback
            { "traceback", string.IsNullOrEmpty(kwResult.KeywordTraceback) ? "" : kwResult.KeywordTraceback },
            // Add output
            { "output", string.IsNullOrEmpty(kwResult.KeywordOutput) ? "" : kwResult.KeywordOutput }
        };
        // Add return
        //64bit int has to be returned as string
        if (kwResult.KeywordReturn != null)
        {
            result.Add("return",
                kwResult.KeywordReturn is long ? kwResult.KeywordReturn.ToString() : kwResult.KeywordReturn);
        }
        else
        {
            result.Add("return", "");
        }

        if (kwResult.Status != KeywordResult.KeywordStatus.Fail) return result;
        switch (kwResult.ErrorType)
        {
            case RobotException.RobotExceptionType.Continuable:
                result.Add("continuable", true);
                break;
            case RobotException.RobotExceptionType.Fatal:
                result.Add("fatal", true);
                break;
            case RobotException.RobotExceptionType.Fail: return result;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return result;
    }

    public static XmlRpcStruct FromLibraryObject(Dictionary<string, Dictionary<string, object>> libraryObject)
    {
        var outerStruct = new XmlRpcStruct();
        foreach (var outerItem in libraryObject)
        {
            var innerStruct = new XmlRpcStruct();
                
            foreach (var innerItem in outerItem.Value)
            {
                if (innerItem.Value == null)
                {
                    innerStruct.Add(innerItem.Key, "None");
                    continue;
                }
                innerStruct.Add(innerItem.Key, innerItem.Value);
            }
            outerStruct.Add(outerItem.Key, innerStruct);
        }

        return outerStruct;
    }
}