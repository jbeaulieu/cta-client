using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when the number of successful API requests using the supplied API key
///   have exceeded the maximum daily value.
/// </summary>
public class DailyLimitExceededException : CtaException
{
    public DailyLimitExceededException() : base(ErrorCode.DailyLimitExceeded) { }
    public DailyLimitExceededException(string? message) : base(ErrorCode.DailyLimitExceeded, message) { }
    public DailyLimitExceededException(string? message, Exception? innerException) : base(ErrorCode.DailyLimitExceeded, message, innerException) { }
}
