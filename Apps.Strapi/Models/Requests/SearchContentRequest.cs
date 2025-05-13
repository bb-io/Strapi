using Apps.Strapi.Handlers.Static;
using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Strapi.Models.Requests;

public class SearchContentRequest : ContentTypeIdentifier
{
    [Display("Language", Description = "The language of the content"), StaticDataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }

    [Display("Status", Description = "The status of the content"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }
}