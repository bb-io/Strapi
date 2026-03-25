using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Strapi.Events.Models;

public record PollingSearchContentResponse(List<PollingDocumentResponse> Documents) 
    : IMultiDownloadableContentOutput<PollingDocumentResponse>
{
    [Display("Content")]
    public List<PollingDocumentResponse> Items { get; set; } = Documents;

    [Display("Total count")]
    public double TotalCount { get; set; } = Documents.Count;
}