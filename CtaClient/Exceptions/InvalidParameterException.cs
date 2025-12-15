using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when a supplied parameter is invalid.
/// </summary>
public class InvalidParameterException : CtaBaseException
{
    public InvalidParameterException(ErrorCode errorCode) : base(errorCode) { }
    public InvalidParameterException(ErrorCode errorCode, string? message) : base(errorCode, message) { }
    public InvalidParameterException(ErrorCode errorCode, string? message, Exception? innerException) : base(errorCode, message, innerException) { }
}
