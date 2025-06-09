namespace Apps.Strapi.Constants;

public class GraphQlQueries
{
    private const string GetLocalizationsForContent = @"query {
        {contentType}(id: ""{contentId}"") {
            data {
                attributes {
                    locale
                    localizations {
                        data {
                            id
                            attributes {
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