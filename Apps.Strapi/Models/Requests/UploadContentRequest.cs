using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests;

public class UploadContentRequest : VersionOptionalRequest
{
    [Display("File", Description = "The HTML file to upload.")]
    public FileReference File { get; set; } = default!;

    [Display("Target language", Description = "The target language to upload the content to."), DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;
}