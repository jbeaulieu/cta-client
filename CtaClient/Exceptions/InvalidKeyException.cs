using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when the supplied value for the API key is not valid.
/// </summary>
public class InvalidKeyException : CtaBaseException
{
    public InvalidKeyException() : base(ErrorCode.InvalidApiKey) { }
    public InvalidKeyException(string? message) : base(ErrorCode.InvalidApiKey, message) { }
    public InvalidKeyException(string? message, Exception? innerException) : base(ErrorCode.InvalidApiKey, message, innerException) { }
}
