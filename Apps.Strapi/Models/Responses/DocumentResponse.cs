using Blackbird.Applications.Sdk.Common;

namespace Models.Responses;

public class DocumentResponse
{
    [Display("Content ID")]
    public string? DocumentId { get; set; }

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