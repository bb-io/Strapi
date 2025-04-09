using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Files
{
    public class UpdateFileInfoRequest
    {
        [Display("File DocumentId", Description = "File DocumentId of the resource you want to modify.")]
        public int? Id { get; set; }

        [Display("File Info", Description = "File info to update")]

        public string FileInfo { get; set; } = string.Empty;
    }
}
