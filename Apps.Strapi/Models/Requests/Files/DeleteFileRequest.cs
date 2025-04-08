using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Files
{
    public class DeleteFileRequest
    {
        [Display("File Id", Description = "File Id to delete.")]
        public int? Id { get; set; }
    }
}
