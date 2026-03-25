using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Apps.Strapi.Models.Responses;

namespace Models.Responses;

public class DocumentResponse : BaseDocumentResponse, IContentOutput
{
    [Display("Content ID"), JsonProperty("documentId")]
    public string Id { get; set; } = string.Empty;    
}