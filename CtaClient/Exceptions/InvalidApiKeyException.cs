using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when the supplied value for the API key is not valid.
/// </summary>
public class InvalidApiKeyException : CtaException
{
    public InvalidApiKeyException() : base(ErrorCode.InvalidApiKey) { }
    public InvalidApiKeyException(string? message) : base(ErrorCode.InvalidApiKey, message) { }
    public InvalidApiKeyException(string? message, Exception? innerException) : base(ErrorCode.InvalidApiKey, message, innerException) { }
}
