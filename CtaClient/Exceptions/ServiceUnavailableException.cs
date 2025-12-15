using System.Net;

namespace CtaClient.Exceptions;

/// <summary>
///   Exception throw when a service is not available to serve the incoming request.
/// </summary>
public class ServiceUnavailableException : ServiceException
{
    public ServiceUnavailableException(HttpStatusCode statusCode) : base(statusCode) { }
    public ServiceUnavailableException(HttpStatusCode statusCode, string? message) : base(statusCode, message) { }
    public ServiceUnavailableException(HttpStatusCode statusCode, string? message, Exception? innerException) : base(statusCode, message, innerException) { }
}
