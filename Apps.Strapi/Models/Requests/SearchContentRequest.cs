using Apps.Strapi.Handlers;
using Apps.Strapi.Handlers.Static;
using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class SearchContentRequest
{
    [Display("Content type IDs", Description = "The content types you want to search for.")]
    public IEnumerable<string> ContentTypeIds { get; set; } = [];

    [Display("Language", Description = "The language of the content"), DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }

    [Display("Status", Description = "The status of the content"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }

    [Display("Field names", Description = "The names of the fields to filter the content by.")]
    public IEnumerable<string>? FieldNames { get; set; }

    [Display("Field values", Description = "The values of the fields to filter the content by. Add 'null' to filter by empty values.")]
    public IEnumerable<string>? FieldValues { get; set; }

    [Display("Updated after", Description = "Return only documents updated strictly after this timestamp (UTC).")]
    public DateTime? UpdatedAfter { get; set; }
}