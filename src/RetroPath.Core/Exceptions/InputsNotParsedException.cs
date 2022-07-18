namespace RetroPath.Core.Exceptions;

public class InputsNotParsedException : Exception
{
    public InputsNotParsedException()
    {
    }

    public InputsNotParsedException(string message) : base(message)
    {
    }

    public InputsNotParsedException(string message, Exception inner) : base(message, inner)
    {
    }
}