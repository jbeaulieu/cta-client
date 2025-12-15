using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when too many values were submitted for a single parameter.
/// </summary>
public class MaxValuesExceededException : CtaException
{
    public MaxValuesExceededException(ErrorCode errorCode) : base(errorCode) { }
    public MaxValuesExceededException(ErrorCode errorCode, string? message) : base(errorCode, message) { }
    public MaxValuesExceededException(ErrorCode errorCode, string? message, Exception? innerException) : base(errorCode, message, innerException) { }
}
