using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class GetDocumentsRequest : ApiIdentifier
    {
        [Display("DocumentId", Description ="DocumentId of the documents you want to return")] //TODO this might not be needed.
        public string? DocumentId { get; set; }
    }
}
