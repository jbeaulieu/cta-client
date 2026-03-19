namespace CtaClient.Exceptions;

/// <summary>
///   Base exception for all errors emitted by the CTA Client.
/// </summary>
public class CtaClientException : Exception
{
    public CtaClientException(): base() { }

    public CtaClientException(string? message) : base(message) { }

    public CtaClientException(string? message, Exception? innerException) : base(message, innerException) { }
}
