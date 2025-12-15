using CtaClient.Models;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when an unknown error occurs on the CTA API, preventing a successful response.
/// </summary>
public class ServerErrorException : CtaException
{
    public ServerErrorException() : base(ErrorCode.ServerError) { }
    public ServerErrorException(string? message) : base(ErrorCode.ServerError, message) { }
    public ServerErrorException(string? message, Exception? innerException) : base(ErrorCode.ServerError, message, innerException) { }
}
