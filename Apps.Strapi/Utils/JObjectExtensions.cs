using Apps.Strapi.Models.Responses;
using Models.Responses;
using Newtonsoft.Json.Linq;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Strapi.Utils;

public static class JObjectExtensions
{
    public static List<DocumentResponse> ToContentListResponse(this List<JObject> jObjects)
    {
        var result = new List<DocumentResponse>();
        foreach (var jObject in jObjects)
        {
            var contentResponse = jObject.ToContentResponse();
            result.Add(contentResponse);
        }

        return result;
    }

    public static DocumentResponse ToContentResponse(this JObject jObject)
    {
        return ParseContentObject(jObject);
    }

    public static DocumentWithLocalizationsResponse ToContentWithLocalizationsResponse(this JObject jObject)
    {
        var documentResponse = ParseContentObject(jObject);
        var result = new DocumentWithLocalizationsResponse(documentResponse);
        
        JObject contentObject = jObject;
        if (jObject["data"] != null && jObject["data"]!.Type == JTokenType.Object)
        {
            contentObject = jObject["data"] as JObject ?? throw new Exception("Failed to parse content data from response.");
        }
        
        if (contentObject == null)
        {
            throw new PluginApplicationException("Failed to find content data in response.");
        }
        
        var localizationsToken = GetCaseInsensitiveValue(contentObject, "localizations");
        if (localizationsToken == null)
        {
            throw new PluginApplicationException(
                "Localizations property not found. Please verify your Strapi version - by default this action assumes v5, but you might be using v4.");
        }
        
        if (localizationsToken.Type == JTokenType.Array)
        {
            var localizationsArray = localizationsToken as JArray;
            if (localizationsArray != null)
            {
                foreach (var item in localizationsArray)
                {
                    if (item.Type == JTokenType.Object)
                    {
                        var localization = ParseContentObject((item as JObject)!);
                        result.Localizations.Add(localization);
                    }
                }
            }
        }
        
        return result;
    }
    
    public static DocumentResponse ToFullContentResponse(this JObject jObject)
    {
        if (jObject["data"] != null)
        {
            var dataObject = jObject["data"] as JObject;
            if (dataObject != null)
            {
                return dataObject.ToContentResponse();
            }
        }

        return jObject.ToContentResponse();
    }
    
    private static DocumentResponse ParseContentObject(JObject contentObject)
    {
        var response = new DocumentResponse();
        
        if (contentObject["attributes"] != null && contentObject["attributes"]!.Type == JTokenType.Object)
        {
            response.DocumentId = contentObject["id"]?.ToString();
            var attributesObj = contentObject["attributes"] as JObject;

            if (attributesObj == null)
            {
                return response;
            }

            response.DocumentId = GetCaseInsensitiveValue(attributesObj, "documentId")?.ToString() ?? response.DocumentId;

            response.Title = GetCaseInsensitiveValue(attributesObj, "name")?.ToString() ??
                           GetCaseInsensitiveValue(attributesObj, "title")?.ToString();

            response.CreatedAt = ParseDateTime(GetCaseInsensitiveValue(attributesObj, "createdAt")) ?? DateTime.MinValue;
            response.UpdatedAt = ParseDateTime(GetCaseInsensitiveValue(attributesObj, "updatedAt")) ?? DateTime.MinValue;
            response.PublishedAt = ParseDateTime(GetCaseInsensitiveValue(attributesObj, "publishedAt"));
            response.Locale = GetCaseInsensitiveValue(attributesObj, "locale")?.ToString();
        }
        else
        {
            response.DocumentId = GetCaseInsensitiveValue(contentObject, "id")?.ToString();
            response.DocumentId = GetCaseInsensitiveValue(contentObject, "documentId")?.ToString() ?? response.DocumentId;

            response.Title = GetCaseInsensitiveValue(contentObject, "name")?.ToString() ??
                           GetCaseInsensitiveValue(contentObject, "title")?.ToString();

            response.CreatedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "createdAt")) ?? DateTime.MinValue;
            response.UpdatedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "updatedAt")) ?? DateTime.MinValue;
            response.PublishedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "publishedAt"));
            response.Locale = GetCaseInsensitiveValue(contentObject, "locale")?.ToString();
        }
        
        return response;
    }
    
    private static JToken? GetCaseInsensitiveValue(JObject jObject, string propertyName)
    {
        if (jObject == null)
        {
            return null;
        }

        if (jObject[propertyName] != null)
        {
            return jObject[propertyName];
        }
        
        foreach (var property in jObject.Properties())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }
        }
        
        return null;
    }
    
    private static DateTime? ParseDateTime(JToken? token)
    {
        if (token == null)
        {
            return null;
        }

        if (DateTime.TryParse(token.ToString(), out DateTime result))
        {
            return result;
        }
        
        return null;
    }
}