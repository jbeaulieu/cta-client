using System.Net;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when an error is caused by invalid client input.
/// </summary>
public class NotFoundException : ServiceException
{
    public NotFoundException(HttpStatusCode statusCode) : base(statusCode) { }
    public NotFoundException(HttpStatusCode statusCode, string? message) : base(statusCode, message) { }
    public NotFoundException(HttpStatusCode statusCode, string? message, Exception? innerException) : base(statusCode, message, innerException) { }
}
