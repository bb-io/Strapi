using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class DeleteDocumentRequest : ApiIdentifier
    {
        [Display("DocumentId", Description = "Description")]
        public string? DocumentId { get; set; }
    }
}