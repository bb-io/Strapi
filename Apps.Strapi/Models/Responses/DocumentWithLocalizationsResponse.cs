using Models.Responses;

namespace Apps.Strapi.Models.Responses;

public class DocumentWithLocalizationsResponse : DocumentResponse
{
    public DocumentWithLocalizationsResponse() { }
    
    public DocumentWithLocalizationsResponse(DocumentResponse documentResponse)
    {
        Id = documentResponse.Id;
        Title = documentResponse.Title;
        CreatedAt = documentResponse.CreatedAt;
        UpdatedAt = documentResponse.UpdatedAt;
        PublishedAt = documentResponse.PublishedAt;
        Locale = documentResponse.Locale;
    }
    
    public List<DocumentResponse> Localizations { get; set; } = [];
}
