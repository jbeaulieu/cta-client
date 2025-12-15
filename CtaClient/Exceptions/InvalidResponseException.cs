namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when an error is caused by an invalid CTA API response
/// </summary>
public class InvalidResponseException : Exception
{
    public InvalidResponseException() { }
    public InvalidResponseException(string? message) : base(message) { }
    public InvalidResponseException(string? message, Exception? innerException) : base(message, innerException) { }
}
