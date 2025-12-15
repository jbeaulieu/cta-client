using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   General exception for all errors caused by the CTA API.
/// </summary>
public class CtaBaseException : Exception
{
    public ErrorCode ErrorCode { get; protected set; }

    public CtaBaseException(ErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public CtaBaseException(ErrorCode errorCode, string? message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public CtaBaseException(ErrorCode errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
