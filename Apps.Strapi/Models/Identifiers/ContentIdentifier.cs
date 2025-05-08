using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Identifiers;

public class ContentIdentifier : ContentTypeIdentifier
{
    [Display("Content ID", Description = "The content ID of the document"), DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; } = string.Empty;
}