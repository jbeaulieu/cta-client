using System.Net;

namespace CtaClient.Exceptions;

/// <summary>
///   General exception for all errors caused by an unsuccessful HTTP call to the CTA API.
/// </summary>
public class ServiceException : Exception
{
    public HttpStatusCode ErrorCode { get; protected set; }

    public ServiceException(HttpStatusCode statusCode)
    {
        ErrorCode = statusCode;
    }

    public ServiceException(HttpStatusCode statusCode, string? message) : base(message)
    {
        ErrorCode = statusCode;
    }

    public ServiceException(HttpStatusCode statusCode, string? message, Exception? innerException) : base(message, innerException)
    {
        ErrorCode = statusCode;
    }
}
