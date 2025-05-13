using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Strapi.Models.Requests;

public class ContentStatusOptionalRequest
{    
    [Display("Status", Description = "The status of the content, either 'draft' and 'published'"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }
}
