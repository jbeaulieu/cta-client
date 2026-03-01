using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when predictions for a train are unavailable through the "Follow This Train API".
/// </summary>
public class PredictionException : CtaException
{
    public PredictionException(ErrorCode errorCode) : base(errorCode) { }
    public PredictionException(ErrorCode errorCode, string? message) : base(errorCode, message) { }
    public PredictionException(ErrorCode errorCode, string? message, Exception? innerException) : base(errorCode, message, innerException) { }
}
