namespace Apps.Strapi.Constants;

public static class JsonProperties
{
    public static readonly HashSet<string> NonLocalizableProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "documentId", "createdAt", "updatedAt", "locale"
    };
}