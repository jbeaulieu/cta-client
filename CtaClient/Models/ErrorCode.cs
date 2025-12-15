namespace CtaClient.Models;

/// <summary>
///   Enum mapping of numeric error codes provided by the CTA Train Tracker API.
///   See https://www.transitchicago.com/developers/ttdocs/#_Toc296199912 for more information.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    ///   No error
    /// </summary>
    None = 0,

    /// <summary>
    ///   The query string does not contain one of the required parameters
    /// </summary>
    MissingParameter = 100,

    /// <summary>
    ///   The API key provided is not valid
    /// </summary>
    InvalidApiKey = 101,

    /// <summary>
    ///   The number of successful API Requests using the supplied key have exceeded the maximum daily value
    /// </summary>
    DailyLimitExceeded = 102,

    /// <summary>
    ///   At least one of the supplied values for the "mapid" parameter is not valid
    /// </summary>
    InvalidMapId = 103,

    /// <summary>
    ///   At least one of the supplied values for the "mapid" parameter is not an integer
    /// </summary>
    MapIdNotInteger = 104,

    /// <summary>
    ///   More than 4 values were supplied for the MapId parameter
    /// </summary>
    MaxMapIdsExceeded = 105,

    /// <summary>
    ///   At least one of the supplied values for the Route parameter is invalid
    /// </summary>
    InvalidRoute = 106,

    /// <summary>
    ///   More than 4 values were supplied for the Route parameter
    /// </summary>
    MaxRoutesExceeded = 107,

    /// <summary>
    ///   At least one of the supplied values for the StopId parameter is invalid
    /// </summary>
    InvalidStopId = 108,

    /// <summary>
    ///   More than 4 values were supplied for the StopId parameter
    /// </summary>
    MaxStopIdsExceeded = 109,

    /// <summary>
    ///   A non-integer value was specified for the MaxResults parameter
    /// </summary>
    InvalidMaxParam = 110,

    /// <summary>
    ///   A value less than 1 was specified for the MaxResults parameter
    /// </summary>
    NonPositiveMaxParam = 111,

    /// <summary>
    ///   At least one of the supplied values for the StopId parameter is not an integer
    /// </summary>
    NonIntegerStopId = 112,

    /// <summary>
    ///   The query contains a parameter that is not supported by the train tracker API
    /// </summary>
    InvalidParameter = 500,

    /// <summary>
    ///   A server error occurred
    /// </summary>
    ServerError = 900
}
