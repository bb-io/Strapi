using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Events.Models;

public class SearchContentWithTypeResponse(List<DocumentWithContentTypeResponse> documents)
{
    [Display("Content")]
    public List<DocumentWithContentTypeResponse> Content { get; set; } = documents;

    [Display("Total count")]
    public double TotalCount { get; set; } = documents.Count;
}