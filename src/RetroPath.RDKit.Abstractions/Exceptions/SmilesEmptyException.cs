namespace RetroPath.RDKit.Abstractions.Exceptions;

public class SmilesEmptyException : Exception
{
    public SmilesEmptyException()
    {
    }

    public SmilesEmptyException(string message) : base(message)
    {
    }

    public SmilesEmptyException(string message, Exception inner) : base(message, inner)
    {
    }
}