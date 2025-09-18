using Apps.Strapi.Models.Responses;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Events.Models;

public class SearchContentWithTypeResponse(List<DocumentWithContentTypeResponse> documents) : ISearchContentOutput<DocumentWithContentTypeResponse>
{
    [Display("Content")]
    public IEnumerable<DocumentWithContentTypeResponse> Items { get; set; } = documents;

    [Display("Total count")]
    public double TotalCount { get; set; } = documents.Count;
}