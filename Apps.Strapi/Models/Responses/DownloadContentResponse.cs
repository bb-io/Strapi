using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Responses;

public class DownloadContentResponse(FileReference fileReference, string contentTypeId) : FileResponse(fileReference)
{
    [Display("Content type ID")]
    public string ContentTypeId { get; set; } = contentTypeId;
}