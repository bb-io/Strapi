namespace Apps.Strapi.Models.Records;

public record HtmlGenerationMetadata(
    string? ContentId,
    string ContentTypeId,
    string Locale,
    string? Ucid,
    string? ContentName,
    string? AdminUrl,
    string BaseUrl,
    string? UpdatedBy = null);
