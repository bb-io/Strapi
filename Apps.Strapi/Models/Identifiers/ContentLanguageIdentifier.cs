using Apps.Strapi.Handlers;
using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Identifiers;

public class ContentLanguageIdentifier : ContentIdentifier
{
    [Display("Language", Description = "The language of the content"), DataSource(typeof(LanguageDataHandler))]
    public string Language { get; set; } = string.Empty;
}
