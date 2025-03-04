using RobotCore.Library.Utility;

namespace RobotCore.Library.Exceptions;

public class RobotException : Exception
{
    public RobotExceptionType ExceptionType { get; } = RobotExceptionType.Fail;
    
    #region ctor

    public RobotException() : base()
    {
    }
    
    public RobotException(string message, RobotException.RobotExceptionType type) : base(message)
    {
    }
    
    public RobotException(string message, Exception innerException, RobotException.RobotExceptionType type) : base(message, innerException)
    {
    }

    #endregion

    public override string ToString()
    {
        return $"[RobotException {ExceptionType}] {Message}";
    }

    #region NestedTypes
    
    public enum RobotExceptionType
    {
        [StringValue("FAIL")]Fail,
        [StringValue("CONTINUABLE")]Continuable,
        [StringValue("FATAL")]Fatal
    }
    
    public class FailKeywordException(string message) : RobotException(message, RobotExceptionType.Fail);

    public class ContinuableKeywordException(string message) : RobotException(message, RobotExceptionType.Continuable);
    
    public class FatalKeywordException(string message) : RobotException(message, RobotExceptionType.Fatal);

    #endregion
}