using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class GetDocumentRequest : ApiIdentifier
    {
        [Display("Document DocumentId", Description = "DocumentId of the document you want to return.")]
        public string? DocumentId { get; set; }
    }
}