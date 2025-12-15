using System.Net;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception thrown when an error is caused by a service request timeout.
/// </summary>
public class ServiceTimedOutException : ServiceException
{
    public ServiceTimedOutException(HttpStatusCode statusCode) : base(statusCode) { }
    public ServiceTimedOutException(HttpStatusCode statusCode, string? message) : base(statusCode, message) { }
    public ServiceTimedOutException(HttpStatusCode statusCode, string? message, Exception? innerException) : base(statusCode, message, innerException) { }
}
