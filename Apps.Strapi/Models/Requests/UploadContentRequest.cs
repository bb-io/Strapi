using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests;

public class UploadContentRequest
{
    [Display("File", Description = "The HTML file to upload.")]
    public FileReference File { get; set; } = default!;

    [Display("Target language", Description = "The target language to upload the content to."), StaticDataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Strapi version", Description = "The version of Strapi to use for the upload. By default we will use version 5."), StaticDataSource(typeof(StrapiVersionDataHandler))]
    public string? StrapiVersion { get; set; }
}