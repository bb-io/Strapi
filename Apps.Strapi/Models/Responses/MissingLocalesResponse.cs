using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Models.Responses;

public class MissingLocalesResponse(IEnumerable<string> missingLocales, IEnumerable<string> supportedLocales)
{
    [Display("Missing locales")]
    public List<string> MissingLocales { get; set; } = missingLocales.ToList();

    [Display("Supported locales")]
    public List<string> SupportedLocales { get; set; } = supportedLocales.ToList();

    public static List<string> GetLocalesFromJObject(JObject jObject)
    {
        var locales = new List<string>();

        try
        {
            foreach (var item in GetLocalizationEntries(jObject))
            {
                var currentLocale = item["attributes"]?["locale"]?.ToString() ?? item["locale"]?.ToString();
                if (!string.IsNullOrEmpty(currentLocale))
                {
                    locales.Add(currentLocale);
                }
            }
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException("Failed to extract locales from the provided JSON object. Please, ask Blackbird support for further investigation", ex);
        }

        return locales.Distinct().ToList();
    }
    
    public static List<IdWithLocale> GetIdsWithLocalesFromJObject(JObject jObject)
    {
        var idsWithLocales = new List<IdWithLocale>();

        try
        {
            foreach (var item in GetLocalizationEntries(jObject))
            {
                var currentLocale = item["attributes"]?["locale"]?.ToString() ?? item["locale"]?.ToString();
                var currentId = item["id"]?.ToString();
                if (!string.IsNullOrEmpty(currentLocale) && !string.IsNullOrEmpty(currentId))
                {
                    idsWithLocales.Add(new IdWithLocale(currentId, currentLocale));
                }
            }
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException("Failed to extract locales from the provided JSON object. Please, ask Blackbird support for further investigation", ex);
        }

        return idsWithLocales.Distinct().ToList();
    }

    public static List<string> GetMissingLocales(IEnumerable<string> existingLocales, IEnumerable<string> targetLocales)
    {
        return targetLocales
            .Where(locale => !existingLocales.Contains(locale))
            .ToList();
    }

    private static IEnumerable<JObject> GetLocalizationEntries(JObject jObject)
    {
        var contentData = jObject["data"] as JObject;
        if (contentData == null)
        {
            return [];
        }

        var entries = new List<JObject> { contentData };
        var localizationsData = contentData["attributes"]?["localizations"]?["data"] ?? contentData["localizations"];

        if (localizationsData is JArray localizationsArray)
        {
            entries.AddRange(localizationsArray.OfType<JObject>());
        }

        return entries;
    }
}

public record IdWithLocale(string Id, string Locale);
