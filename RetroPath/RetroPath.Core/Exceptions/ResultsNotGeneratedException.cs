namespace RetroPath.Core.Exceptions;

public class ResultsNotGeneratedException : Exception
{
    public ResultsNotGeneratedException()
    {
    }

    public ResultsNotGeneratedException(string message) : base(message)
    {
    }

    public ResultsNotGeneratedException(string message, Exception inner) : base(message, inner)
    {
    }
}