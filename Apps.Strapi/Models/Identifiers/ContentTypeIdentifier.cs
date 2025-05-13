using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Identifiers;

public class ContentTypeIdentifier
{
    [Display("Content type ID", Description = "The content type of the document")]
    public string ContentTypeId { get; set; } = string.Empty;
}