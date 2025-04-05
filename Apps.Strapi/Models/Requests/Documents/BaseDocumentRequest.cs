using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests.Documents
{
    public class BaseDocumentRequest //TODO maybe move this to identifiers?
    {
        [Display("Api Id", Description = "Description")]
        public string ApiId { get; set; }
    }
}