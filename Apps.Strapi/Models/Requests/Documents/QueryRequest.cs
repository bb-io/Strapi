using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class QueryRequest
    {
        [Display("Raw query", Description="Custom raw query (eg. built with interactive query builder.")]
        string? Query { get; set; } = string.Empty;
    }
}
