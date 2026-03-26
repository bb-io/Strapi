using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Constants;

public static class JsonProperties
{
    public static readonly HashSet<string> NonLocalizableProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "documentId", "createdAt", "updatedAt", "locale"
    };

    public static readonly HashSet<string> RoundTripIgnoredProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "localizations"
    };

    public static void RemoveRoundTripIgnoredProperties(JToken token)
    {
        if (token is JObject obj)
        {
            var propertiesToRemove = obj.Properties()
                .Where(p => RoundTripIgnoredProperties.Contains(p.Name))
                .ToList();

            foreach (var property in propertiesToRemove)
            {
                property.Remove();
            }

            foreach (var property in obj.Properties().ToList())
            {
                RemoveRoundTripIgnoredProperties(property.Value);
            }

            return;
        }

        if (token is JArray array)
        {
            foreach (var item in array)
            {
                RemoveRoundTripIgnoredProperties(item);
            }
        }
    }
}
