using Blackbird.Applications.Sdk.Common;
using Models.Responses;

namespace Apps.Strapi.Models.Responses;

public class DocumentWithContentTypeResponse : DocumentResponse
{
    [Display("Content type ID", Description = "The ID of the content type to which this content belongs.")]
    public string ContentTypeId { get; set; } = string.Empty;

    public DocumentWithContentTypeResponse() { }

    public DocumentWithContentTypeResponse(DocumentResponse documentResponse, string contentTypeId)
    {
        Id = documentResponse.Id;
        Title = documentResponse.Title;
        CreatedAt = documentResponse.CreatedAt;
        UpdatedAt = documentResponse.UpdatedAt;
        PublishedAt = documentResponse.PublishedAt;
        Locale = documentResponse.Locale;
        ContentTypeId = contentTypeId;
    }
}