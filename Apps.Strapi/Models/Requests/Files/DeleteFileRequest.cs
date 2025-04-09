using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Files
{
    public class DeleteFileRequest
    {
        [Display("File DocumentId", Description = "File DocumentId to delete.")]
        public int? Id { get; set; }
    }
}
