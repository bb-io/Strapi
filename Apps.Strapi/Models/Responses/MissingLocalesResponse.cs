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

    public static List<string> GetLocalesFromJObject(JObject jObject, string contentType)
    {
        var locales = new List<string>();

        try
        {
            var localizationsData = jObject["data"]?[contentType]?["data"]?["attributes"]?["localizations"]?["data"];

            if (localizationsData != null && localizationsData.Type == JTokenType.Array)
            {
                foreach (var item in localizationsData)
                {
                    var locale = item["attributes"]?["locale"]?.ToString();
                    if (!string.IsNullOrEmpty(locale))
                    {
                        locales.Add(locale);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException("Failed to extract locales from the provided JSON object. Please, ask Blackbird support for further investigation", ex);
        }

        return locales;
    }

    public static List<string> GetMissingLocales(IEnumerable<string> existingLocales, IEnumerable<string> targetLocales)
    {
        return targetLocales
            .Where(locale => !existingLocales.Contains(locale))
            .ToList();
    }
}