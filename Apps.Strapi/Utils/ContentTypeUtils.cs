namespace Apps.Strapi.Utils;

public static class ContentTypeUtils
{
    public static string ConvertToGraphQlContentType(string contentType)
    {
        var parts = contentType.Split('-');
        var graphQlContentType = parts[0].ToLower();
        for (int i = 1; i < parts.Length; i++)
        {
            graphQlContentType += char.ToUpper(parts[i][0]) + parts[i].Substring(1).ToLower();
        }
        
        return graphQlContentType.EndsWith('s') ? graphQlContentType[..^1] : graphQlContentType;
    }
}