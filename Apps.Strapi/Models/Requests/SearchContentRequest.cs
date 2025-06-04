using Apps.Strapi.Handlers;
using Apps.Strapi.Handlers.Static;
using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class SearchContentRequest : ContentTypeIdentifier
{
    [Display("Language", Description = "The language of the content"), DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }

    [Display("Status", Description = "The status of the content"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }

    [Display("Field paths", Description = "The fields to filter the content by. If not specified, all fields will be returned."), DataSource(typeof(StringFieldPathDataHandler))]
    public IEnumerable<string>? FieldPaths { get; set; }

    [Display("Field values", Description = "The values of the fields to filter the content by.")]
    public IEnumerable<string>? FieldValues { get; set; }
}