using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class DownloadContentRequest
{
    [Display("Exclude fields", Description = "List of fields to exclude from the HTML."), DataSource(typeof(ExcludeFieldsDataHandler))]
    public IEnumerable<string>? ExcludeFields { get; set; }
}