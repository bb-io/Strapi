using Apps.Strapi.Handlers;
using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Events.Models;

public class ContentFilters
{
    [Display("Content type IDs", Description = "The content type of the document")]
    public IEnumerable<string> ContentTypeIds { get; set; } = new List<string>();

    [Display("Language", Description = "The language of the content"), DataSource(typeof(SearchLanguageDataHandler))]
    public string? Language { get; set; }

    [Display("Status", Description = "The status of the content"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }

    [Display("Field names", Description = "The names of the fields to filter the content by.")]
    public IEnumerable<string>? FieldNames { get; set; }

    [Display("Field values", Description = "The values of the fields to filter the content by.")]
    public IEnumerable<string>? FieldValues { get; set; }
}