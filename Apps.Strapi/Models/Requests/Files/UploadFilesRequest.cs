using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests.Files
{
    public class UploadFilesRequest
    {
        [Display("Files", Description = "Files to upload.")]
        public FileReference[]? Files { get; set; } = Array.Empty<FileReference>();
    }
}
