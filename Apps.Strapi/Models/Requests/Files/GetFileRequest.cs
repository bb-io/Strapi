using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Files
{
    public class GetFileRequest
    {
        [Display("File ID", Description= "File DocumentId to list.")]
        public int? Id { get; set; }
    }
}
