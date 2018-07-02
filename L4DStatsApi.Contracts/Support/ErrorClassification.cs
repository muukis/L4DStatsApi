using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace L4DStatsApi.Support
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorClassification
    {
        InvalidArgument,
        EntityNotFound,
        UniqueConstraint,
        ReferenceConstraint,
        AuthenticationError,
        AuthorizationError,
        InvalidOperation,
        InternalError,
    }
}
