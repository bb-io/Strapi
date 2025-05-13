using Blackbird.Applications.Sdk.Common;
using Models.Responses;

namespace Apps.Strapi.Models.Responses;

public class SearchContentResponse(List<DocumentResponse> documents)
{
    [Display("Content")]
    public List<DocumentResponse> Content { get; set; } = documents;

    [Display("Total count")]
    public double TotalCount { get; set; } = documents.Count;
}