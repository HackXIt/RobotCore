namespace RobotCore.Library.Exceptions;

public class LibraryException : Exception
{
    public LibraryException()
    {
    }

    public LibraryException(string? message) : base(message)
    {
    }

    public LibraryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    #region NestedTypes

    public class LoadingException : LibraryException
    {
        public LoadingException()
        {
        }

        public LoadingException(string? message) : base(message)
        {
        }

        public LoadingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    #endregion NestedTypes
}