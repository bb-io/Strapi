using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Responses;

public class BaseDocumentResponse
{
    [Display("Content type ID")]
    public string? ContentTypeId { get; set; }

    [Display("Title")]
    public string? Title { get; set; }

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }

    [Display("Published at")]
    public DateTime? PublishedAt { get; set; }

    [Display("Language")]
    public string? Locale { get; set; }
}
