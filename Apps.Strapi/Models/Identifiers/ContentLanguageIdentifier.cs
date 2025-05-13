using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Strapi.Models.Identifiers;

public class ContentLanguageIdentifier : ContentIdentifier
{
    [Display("Language", Description = "The language of the content"), StaticDataSource(typeof(LanguageDataHandler))]
    public string Language { get; set; } = string.Empty;
}
