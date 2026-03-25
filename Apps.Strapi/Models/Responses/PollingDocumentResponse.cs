using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Models.Responses;
using Newtonsoft.Json;

namespace Apps.Strapi.Models.Responses;

public class PollingDocumentResponse(DocumentResponse documentResponse) : BaseDocumentResponse, IDownloadContentInput
{
    [Display("Content ID"), JsonProperty("documentId")]
    public string ContentId { get; set; } = documentResponse.Id;
}
