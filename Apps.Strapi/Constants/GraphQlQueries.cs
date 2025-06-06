namespace Apps.Strapi.Constants;

public class GraphQlQueries
{
    private const string GetLocalizationsForContent = @"query {
        {contentType}(id: ""{contentId}"") {
            data {
                attributes {
                    localizations {
                        data {
                            id
                            attributes {
                                title
                                locale
                            }
                        }
                    }
                }
            }
        }
    }";

    public static string GetLocalizationObjectsForContentQuery(string contentType, string contentId)
    {
        return GetLocalizationsForContent
            .Replace("{contentType}", contentType)
            .Replace("{contentId}", contentId);
    }
}