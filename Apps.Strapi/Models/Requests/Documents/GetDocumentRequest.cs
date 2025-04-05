using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class GetDocumentRequest : BaseDocumentRequest
    {
        [Display("Id", Description = "Description")]
        public int? Id { get; set; }
    }
}