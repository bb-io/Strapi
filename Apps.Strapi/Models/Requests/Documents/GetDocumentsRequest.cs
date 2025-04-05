using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class GetDocumentsRequest : BaseDocumentRequest
    {
        [Display("Id", Description ="Description")]
        public int? Id { get; set; }
    }
}
