using Blackbird.Applications.Sdk.Common;

namespace Models.Requests
{
    public class BaseDocumentRequest
    {
        [Display("Api Id", Description = "Description")]
        public string ApiId { get; set; }
    }
}