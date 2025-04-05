using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Files
{
    public class UploadFileInfoRequest   //TODO update this model
    {
        [Display("File Id", Description = "Description")]
        public int? Id { get; set; }
    }
}
