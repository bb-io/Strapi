using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class DownloadContentRequest
{
    [Display("Exclude fields", Description = "List of fields to exclude from the HTML."), DataSource(typeof(ExcludeFieldsDataHandler))]
    public IEnumerable<string>? ExcludeFields { get; set; }
    
    [Display("Unique fields", Description = "In Strapi, some fields are unique and cannot be duplicated. List of fields to completely remove them from translated content."), DataSource(typeof(ExcludeFieldsDataHandler))]
    public IEnumerable<string>? UniqueFields { get; set; }
}