using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class UpdateDocumentRequest : ApiIdentifier
    {
        [Display("DocumentId", Description = "DocumentId of the document")]
        public string? DocumentId { get; set; }

        [Display("Document to upload", Description = "The document to upload.")]

        public FileReference File { get; set; } = new FileReference();
    }
}