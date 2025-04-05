using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests.Files
{
    public class UploadFilesRequest   //TODO update this model
    {
        [Display("Files", Description = "Description")]
        FileReference[]? Files { get; set; } = Array.Empty<FileReference>();
    }
}
