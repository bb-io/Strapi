using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Identifiers;

public class ContentIdentifier : ContentTypeIdentifier
{
    [Display("Content ID", Description = "Optional. The ID of the content to be downloaded. Provide the ID if your content type is a collection type."), DataSource(typeof(ContentDataHandler))]
    public string? ContentId { get; set; } 
}