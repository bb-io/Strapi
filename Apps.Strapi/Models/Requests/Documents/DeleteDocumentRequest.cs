using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class DeleteDocumentRequest : ApiIdentifier
    {
        [Display("Id", Description = "Description")]
        public int? Id { get; set; }
    }
}