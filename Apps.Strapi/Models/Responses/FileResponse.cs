using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Responses;

public class FileResponse(FileReference fileReference) : IDownloadContentOutput
{
    public FileReference Content { get; set; } = fileReference;
}