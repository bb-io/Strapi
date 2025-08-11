using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class GetMissingLocalesRequest : VersionOptionalRequest
{
    [Display("Content type ID", Description = "The content type of the document. If you are using strapi v4 this should be the singular form of the content type, e.g., 'article' for 'articles'.")]
    public string ContentTypeId { get; set; } = string.Empty;

    [Display("Content ID", Description = "The ID of the content to be downloaded."), DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; } = string.Empty;
}