using Apps.Strapi.Models.Responses;
using Models.Responses;
using Newtonsoft.Json.Linq;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Strapi.Utils;

public static class JObjectExtensions
{
    public static List<DocumentResponse> ToContentListResponse(this List<JObject> jObjects, string contentTypeId)
    {
        var result = new List<DocumentResponse>();

        foreach (var rootObject in jObjects)
        {
            foreach (var contentObject in ExtractContentObjects(rootObject))
            {
                result.Add(ParseContentObject(contentObject, contentTypeId));
            }
        }

        return result;
    }

    public static DocumentResponse ToContentResponse(this JObject jObject, string contentTypeId)
    {
        var contentObject = ExtractSingleContentObject(jObject);
        return ParseContentObject(contentObject, contentTypeId);
    }

    public static DocumentResponse ToFullContentResponse(this JObject jObject, string contentTypeId)
    {
        var contentObject = ExtractSingleContentObject(jObject);
        return ParseContentObject(contentObject, contentTypeId);
    }

    public static DocumentWithLocalizationsResponse ToContentWithLocalizationsResponse(this JObject jObject,
        string contentTypeId)
    {
        var contentObject = ExtractSingleContentObject(jObject);
        var documentResponse = ParseContentObject(contentObject, contentTypeId);
        var result = new DocumentWithLocalizationsResponse(documentResponse);

        var localizationsToken = GetLocalizationsToken(contentObject);

        if (localizationsToken == null)
        {
            return result;
        }

        foreach (var localizationObject in ExtractContentObjectsFromToken(localizationsToken))
        {
            result.Localizations.Add(ParseContentObject(localizationObject, contentTypeId));
        }

        return result;
    }

    private static List<JObject> ExtractContentObjects(JObject rootObject)
    {
        if (rootObject == null)
        {
            return [];
        }

        if (rootObject["data"] is JArray dataArray)
        {
            return dataArray.OfType<JObject>().ToList();
        }

        if (rootObject["data"] is JObject dataObject)
        {
            return [dataObject];
        }

        return [rootObject];
    }

    private static JObject ExtractSingleContentObject(JObject rootObject)
    {
        if (rootObject["data"] is JObject dataObject)
        {
            return dataObject;
        }

        if (rootObject["data"] is JArray dataArray)
        {
            var firstObject = dataArray.OfType<JObject>().FirstOrDefault();
            if (firstObject != null)
            {
                return firstObject;
            }

            throw new PluginApplicationException("Response contains an empty data array.");
        }

        return rootObject;
    }

    private static IEnumerable<JObject> ExtractContentObjectsFromToken(JToken token)
    {
        if (token is JArray array)
        {
            return array.OfType<JObject>();
        }

        if (token is JObject obj)
        {
            if (obj["data"] is JArray nestedDataArray)
            {
                return nestedDataArray.OfType<JObject>();
            }

            if (obj["data"] is JObject nestedDataObject)
            {
                return [nestedDataObject];
            }

            return [obj];
        }

        return Enumerable.Empty<JObject>();
    }

    private static JToken? GetLocalizationsToken(JObject contentObject)
    {
        // Strapi v4: localizations usually lives in attributes.localizations
        if (contentObject["attributes"] is JObject attributes)
        {
            var localizations = GetCaseInsensitiveValue(attributes, "localizations");
            if (localizations != null)
            {
                return localizations;
            }
        }

        // Strapi v5 / flat variants
        return GetCaseInsensitiveValue(contentObject, "localizations");
    }

    private static DocumentResponse ParseContentObject(JObject contentObject, string contentTypeId)
    {
        var response = new DocumentResponse
        {
            ContentTypeId = contentTypeId
        };

        // Strapi v4 style: { id, attributes: { ... } }
        if (contentObject["attributes"] is JObject attributesObj)
        {
            response.Id = GetCaseInsensitiveValue(contentObject, "id")?.ToString();
            response.Id = GetCaseInsensitiveValue(attributesObj, "documentId")?.ToString() ?? response.Id;

            response.Title =
                GetCaseInsensitiveValue(attributesObj, "name")?.ToString() ??
                GetCaseInsensitiveValue(attributesObj, "title")?.ToString() ??
                GetCaseInsensitiveValue(attributesObj, "symbol")?.ToString();

            response.CreatedAt =
                ParseDateTime(GetCaseInsensitiveValue(attributesObj, "createdAt")) ?? DateTime.MinValue;
            response.UpdatedAt =
                ParseDateTime(GetCaseInsensitiveValue(attributesObj, "updatedAt")) ?? DateTime.MinValue;
            response.PublishedAt = ParseDateTime(GetCaseInsensitiveValue(attributesObj, "publishedAt"));
            response.Locale = GetCaseInsensitiveValue(attributesObj, "locale")?.ToString();

            return response;
        }

        // Strapi v5 style / flat object
        response.Id = GetCaseInsensitiveValue(contentObject, "id")?.ToString();
        response.Id = GetCaseInsensitiveValue(contentObject, "documentId")?.ToString() ?? response.Id;

        response.Title =
            GetCaseInsensitiveValue(contentObject, "name")?.ToString() ??
            GetCaseInsensitiveValue(contentObject, "title")?.ToString() ??
            GetCaseInsensitiveValue(contentObject, "symbol")?.ToString();

        response.CreatedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "createdAt")) ?? DateTime.MinValue;
        response.UpdatedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "updatedAt")) ?? DateTime.MinValue;
        response.PublishedAt = ParseDateTime(GetCaseInsensitiveValue(contentObject, "publishedAt"));
        response.Locale = GetCaseInsensitiveValue(contentObject, "locale")?.ToString();

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

        if (DateTime.TryParse(token.ToString(), out var result))
        {
            return result;
        }

        return null;
    }
}