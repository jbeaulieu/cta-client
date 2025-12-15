using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Base exception for all errors emitted by the CTA API.
/// </summary>
public class CtaException : Exception
{
    public ErrorCode ErrorCode { get; protected set; }

    public CtaException(ErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public CtaException(ErrorCode errorCode, string? message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public CtaException(ErrorCode errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
