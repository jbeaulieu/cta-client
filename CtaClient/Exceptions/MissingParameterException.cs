using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when a required parameter is missing.
/// </summary>
public class MissingParameterException : CtaException
{
    public MissingParameterException() : base(ErrorCode.MissingParameter) { }
    public MissingParameterException(string? message) : base(ErrorCode.MissingParameter, message) { }
    public MissingParameterException(string? message, Exception? innerException) : base(ErrorCode.MissingParameter, message, innerException) { }
}
