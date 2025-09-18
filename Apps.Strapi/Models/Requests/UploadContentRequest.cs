using Apps.Strapi.Handlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests;

public class UploadContentRequest : VersionOptionalRequest, IUploadContentInput
{
    [Display("File", Description = "The HTML file to upload.")]
    public FileReference Content { get; set; } = default!;

    [Display("Target language", Description = "The target language to upload the content to."), DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; } = string.Empty;
    
    [Display("Content ID", Description = "The ID of the content to upload the file to. If not provided, a new content entry will be created.")]
    public string? ContentId { get; set; }
}