namespace RetroPath.Core.Exceptions;

public class FingerprintNotCalculatedException : Exception
{
    public FingerprintNotCalculatedException()
    {
    }

    public FingerprintNotCalculatedException(string message) : base(message)
    {
    }

    public FingerprintNotCalculatedException(string message, Exception inner) : base(message, inner)
    {
    }
}