using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class UpdateDocumentRequest : ApiIdentifier
    {
        [Display("Id", Description = "Id of the document")]
        public int? Id { get; set; }

        [Display("Document to upload", Description = "The document to upload.")]

        public FileReference File { get; set; } = new FileReference();
    }
}