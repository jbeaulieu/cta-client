namespace CtaClient.Models;

/// <summary>
///   Represents the error returned by a failed call to the CTA API.
/// </summary>
public sealed record Error(ErrorCode Code, string Description)
{
    public static readonly Error None = new(ErrorCode.None, string.Empty);
}
