using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Models.Responses;
using Newtonsoft.Json;

namespace Apps.Strapi.Models.Responses;

public class PollingDocumentResponse : BaseDocumentResponse, IDownloadContentInput
{
    public PollingDocumentResponse(DocumentResponse documentResponse)
    {
        ContentId = documentResponse.Id;
        ContentTypeId = documentResponse.ContentTypeId;
        Title = documentResponse.Title;
        CreatedAt = documentResponse.CreatedAt;
        UpdatedAt = documentResponse.UpdatedAt;
        PublishedAt = documentResponse.PublishedAt;
        Locale = documentResponse.Locale;
    }

    [Display("Content ID"), JsonProperty("documentId")]
    public string ContentId { get; set; } = string.Empty;
}