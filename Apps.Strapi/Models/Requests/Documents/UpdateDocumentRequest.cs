using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class UpdateDocumentRequest : BaseDocumentRequest //TODO update this model
    {
        [Display("Id", Description = "Description")]
        public int? Id { get; set; }
    }
}