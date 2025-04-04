using Blackbird.Applications.Sdk.Common;

namespace Models.Requests
{
    public class GetDocumentRequest : BaseDocumentRequest
    {
        [Display("Id", Description ="Description")]
        public int? Id { get; set; }
    }
}
