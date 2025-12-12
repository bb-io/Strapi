using Apps.Strapi.Constants;
using Apps.Strapi.Models.Dtos;
using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Apps.Strapi.Models.Responses;
using Apps.Strapi.Utils;
using Apps.Strapi.Utils.Converters;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Apps.Strapi.Models.Records;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Extensions;

namespace Apps.Strapi.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search content", Description = "Returns a list of content based on specified inputs. Only for collection types content types.")]
    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    public async Task<SearchContentResponse> SearchContentAsync([ActionParameter] SearchContentRequest request)
    {
        var allDocuments = new List<DocumentResponse>();
        foreach (var contentTypeId in request.ContentTypeIds)
        {
            var apiRequest = new RestRequest($"/api/{contentTypeId}");
            if (request.Language != null)
            {
                apiRequest.AddQueryParameter("locale", request.Language);
            }

            request.ApplyStatusIfPresent(apiRequest);

            if (request.UpdatedAfter.HasValue)
            {
                var isoUtc = request.UpdatedAfter.Value.ToUniversalTime().ToString("o");
                QueryParameterBuilder.AddFieldFiltersIfAvailable(apiRequest,new[] { "updatedAt" },new[] { isoUtc },"$gt");
            }
            
            if (request.CreatedAfter.HasValue)
            {
                var isoUtc = request.CreatedAfter.Value.ToUniversalTime().ToString("o");
                QueryParameterBuilder.AddFieldFiltersIfAvailable(apiRequest,new[] { "createdAt" },new[] { isoUtc },"$gt");
            }

            QueryParameterBuilder.AddFieldFiltersIfAvailable(apiRequest, request.FieldNames, request.FieldValues);
            
            var result = await Client.PaginateAsync<JObject>(apiRequest);
            allDocuments.AddRange(result.ToContentListResponse(contentTypeId));
        }

        return new(allDocuments);
    }

    [Action("Get missing localization languages", Description = "Returns a list of languages that haven't been localized yet for the specified content.")]
    public async Task<MissingLocalesResponse> GetMissingLocalesAsync([ActionParameter] GetMissingLocalesRequest request)
    {
        ExceptionExtensions.ThrowIfNullOrEmpty(request.ContentTypeId, "Content type ID");
        ExceptionExtensions.ThrowIfNullOrEmpty(request.ContentId, "Content ID");
        request.ContentTypeId = ContentTypeUtils.ConvertToGraphQlContentType(request.ContentTypeId);

        var languages = await GetAllAvailableLanguagesAsync();
        var targetLocales = languages.Select(l => l.Code).ToList();
        if (request.StrapiVersion == StrapiVersions.V4)
        {
            var response = await GetLocalizationObjectsV4Async(request);
            var locales = MissingLocalesResponse.GetLocalesFromJObject(response, request.ContentTypeId);
            var missingLocales = MissingLocalesResponse.GetMissingLocales(locales, targetLocales);
            return new MissingLocalesResponse(missingLocales, locales);
        }
        else
        {
            var content = await GetLocalizationObjectsV5Async(request);
            if (content.Localizations == null || content.Localizations.Count == 0)
            {
                return new MissingLocalesResponse(targetLocales, new List<string>());
            }

            var locales = content.Localizations.Select(l => l.Locale).ToList();
            if (!string.IsNullOrEmpty(content.Locale))
            {
                locales.Add(content.Locale);
            }

            var missingLocales = MissingLocalesResponse.GetMissingLocales(locales!, targetLocales);
            return new MissingLocalesResponse(missingLocales, locales!);
        }
    }

    [Action("Get text field value", Description = "Returns a text field value from a content by ID.")]
    public async Task<StringResponse> GetTextFieldValueAsync([ActionParameter] ContentIdentifier identifier,
        [ActionParameter] GetTextFieldValueRequest request)
    {
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentTypeId, "Content type ID");
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentId, "Content ID");
        ExceptionExtensions.ThrowIfNullOrEmpty(request.FieldPath, "Field path");

        var apiRequest = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}");
        var response = await Client.ExecuteWithErrorHandling(apiRequest);
        var content = response.Content;
        if (content == null)
        {
            throw new PluginMisconfigurationException("Content not found or empty.");
        }

        var jObject = JObject.Parse(content);
        var value = jObject.SelectToken(request.FieldPath)?.ToString() ?? string.Empty;
        return new StringResponse(value);
    }

    [Action("Download content", Description = "Downloads a content by ID. By default  it will download the content for published status")]
    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    public async Task<DownloadContentResponse> DownloadContentAsync([ActionParameter] ContentLanguageIdentifier identifier,
        [ActionParameter] ContentStatusOptionalRequest optionalRequest,
        [ActionParameter] DownloadContentRequest downloadContentRequest)
    {
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentTypeId, "Content type ID");

        var request = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}");
        if (identifier.Language != null)
        {
            request.AddQueryParameter("locale", identifier.Language);
        }

        if (optionalRequest.Status != null)
        {
            request.AddQueryParameter("status", optionalRequest.Status);
        }

        var response = await Client.ExecuteWithErrorHandling(request);
        var responseJson = JObject.Parse(response.Content!);
        var dataObj = responseJson["data"] as JObject;
        
        if (dataObj == null)
        {
            throw new PluginMisconfigurationException("Invalid response structure. Expected 'data' property.");
        }
        
        var contentObj = dataObj["attributes"] as JObject ?? dataObj;
        var locale = GetCaseInsensitiveValue(dataObj, "locale")?.ToString()
                     ?? GetCaseInsensitiveValue(contentObj, "locale")?.ToString() 
                     ?? identifier.Language;
        
        var documentId = GetCaseInsensitiveValue(dataObj, "documentId")?.ToString()
                         ?? GetCaseInsensitiveValue(contentObj, "documentId")?.ToString();
        var id = GetCaseInsensitiveValue(dataObj, "id")?.ToString();
        var ucid = documentId ?? id ?? identifier.ContentId;
        
        var title = GetCaseInsensitiveValue(contentObj, "name")?.ToString()
                    ?? GetCaseInsensitiveValue(contentObj, "title")?.ToString()
                    ?? identifier.ContentId 
                    ?? identifier.ContentTypeId;
        
        var baseUrl = Credentials.First(c => c.KeyName == CredNames.BaseUrl).Value.Trim('/');
        var singularContentType = identifier.ContentTypeId.TrimEnd('s');
        var adminUrl = !string.IsNullOrEmpty(ucid) 
            ? $"{baseUrl}/admin/content-manager/collection-types/api::{singularContentType}.{singularContentType}/{ucid}"
            : null;
        
        var metadata = new HtmlGenerationMetadata(
            ContentId: identifier.ContentId,
            ContentTypeId: identifier.ContentTypeId,
            Locale: locale,
            Ucid: ucid,
            ContentName: title,
            AdminUrl: adminUrl,
            BaseUrl: baseUrl
        );
        
        var htmlString = JsonToHtmlConverter.ConvertToHtml(response.Content!, metadata, downloadContentRequest.ExcludeFields, downloadContentRequest.UniqueFields);
        var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlString))
        {
            Position = 0
        };

        var fileReference = await fileManagementClient.UploadAsync(memoryStream, "text/html", $"{identifier.ContentId}.html");
        return new(fileReference, identifier.ContentTypeId);
    }

    [Action("Upload content", Description = "Uploads a HTML file to a specific language to localize the content.")]
    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    public async Task<UploadContentResponse> UploadContentAsync([ActionParameter] UploadContentRequest request)
    {
        var strapiVersion = request.StrapiVersion ?? "v5";
        if (!StrapiVersions.AllVersions.Contains(strapiVersion))
        {
            var supportedVersions = string.Join(", ", StrapiVersions.AllVersions);
            throw new PluginMisconfigurationException($"Unsupported Strapi version '{strapiVersion}'. Supported versions are: {supportedVersions}.");
        }

        var fileStream = await fileManagementClient.DownloadAsync(request.Content);
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var htmlString = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        
        // Check if input is XLIFF
        var isXliff = Xliff2Serializer.IsXliff2(htmlString);
        Transformation? transformation = null;
        
        if (isXliff)
        {
            transformation = Transformation.Parse(htmlString, request.Content.Name);
            htmlString = transformation.Target().Serialize();
            if (htmlString == null) throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }
        
        var metadata = HtmlToJsonConverter.ExtractMetadata(htmlString);
        if (!string.IsNullOrEmpty(request.ContentId))
        {
            metadata = metadata with { ContentId = request.ContentId };
        }
        
        var jsonContent = HtmlToJsonConverter.ConvertToJson(htmlString, strapiVersion, request.Locale);

        var endpoint = $"/api/{metadata.ContentTypeId}";
        if (!string.IsNullOrEmpty(metadata.ContentId))
        {
            endpoint += $"/{metadata.ContentId}";
        }
        
        DocumentResponse result;
        if (StrapiVersions.V5 == strapiVersion)
        {
            var apiRequest = new RestRequest(endpoint, Method.Put)
                .AddQueryParameter("locale", request.Locale)
                .AddBody(jsonContent, ContentType.Json);

            var jObject = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
            result = jObject.ToFullContentResponse(metadata.ContentTypeId);
        }
        else if (StrapiVersions.V4 == strapiVersion)
        {
            try
            {
                result = await LocalizeV4Async(endpoint, jsonContent, metadata.ContentTypeId);
            }
            catch (Exception e) when (e.Message.Contains("locale is already used"))
            {
                var singularContentTypeId = ContentTypeUtils.ConvertToGraphQlContentType(metadata.ContentTypeId);
                
                result = await HandleV4LocaleAlreadyUsedAsync(
                    singularContentTypeId,
                    metadata.ContentTypeId,
                    metadata.ContentId!,
                    jsonContent,
                    request.Locale);
            }
        }
        else
        {
            throw new PluginMisconfigurationException($"Operation not supported yet for Strapi version '{strapiVersion}'. Ask blackbird support to implement it.");
        }
        
        var uploadContentResponse = new UploadContentResponse
        {
            Id = result.Id,
            ContentTypeId = result.ContentTypeId,
            Title = result.Title,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            PublishedAt = result.PublishedAt,
            Locale = result.Locale
        };
        
        if (isXliff && transformation != null)
        {
            var baseUrl = Credentials.First(c => c.KeyName == CredNames.BaseUrl).Value.Trim('/');
            var singularContentType = metadata.ContentTypeId.TrimEnd('s');
            
            transformation.TargetSystemReference.ContentId = result.Id;
            transformation.TargetSystemReference.ContentName = result.Title ?? metadata.ContentTypeId;
            transformation.TargetSystemReference.AdminUrl = $"{baseUrl}/admin/content-manager/collection-types/api::{singularContentType}.{singularContentType}/{result.Id}";
            transformation.TargetSystemReference.SystemName = "Strapi";
            transformation.TargetSystemReference.SystemRef = baseUrl;
            transformation.TargetLanguage = request.Locale;
            uploadContentResponse.Content = await fileManagementClient.UploadAsync(transformation.Serialize().ToStream(), MediaTypes.Xliff, transformation.XliffFileName);
        }
        else
        {
            uploadContentResponse.Content = request.Content;
        }
        
        return uploadContentResponse;
    }

    [Action("Update content", Description = "Updates a content by ID.")]
    public async Task<DocumentResponse> UpdateContentAsync([ActionParameter] ContentIdentifier identifier,
        [ActionParameter] UpdateContentRequest request)
    {
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentTypeId, "Content type ID");
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentId, "Content ID");

        var dataDictionary = new Dictionary<string, object>();
        for (int i = 0; i < request.FieldNames.Count(); i++)
        {
            var fieldName = request.FieldNames.ElementAt(i);
            var fieldValue = request.FieldValues.ElementAtOrDefault(i) ?? string.Empty;
            dataDictionary[fieldName] = fieldValue;
        }

        var apiRequest = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}", Method.Put)
            .AddJsonBody(new { data = dataDictionary });

        var response = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
        return response.ToFullContentResponse(identifier.ContentTypeId);
    }

    [Action("Delete content", Description = "Deletes a content by ID.")]
    public async Task DeleteContentAsync([ActionParameter] ContentIdentifier request)
    {
        var apiRequest = new RestRequest($"/api/{request.ContentTypeId}/{request.ContentId}", Method.Delete);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }
    
    private static JToken? GetCaseInsensitiveValue(JObject? jObject, string propertyName)
    {
        if (jObject == null)
        {
            return null;
        }
        
        var property = jObject.Properties()
            .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        return property?.Value;
    }
    
    private async Task<DocumentResponse> LocalizeV4Async(string endpoint, string jsonContent, string contentTypeId)
    {
        endpoint += "/localizations";
        var apiRequest = new RestRequest(endpoint, Method.Post)
            .AddStringBody(jsonContent, ContentType.Json);

        var jObject = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
        return jObject.ToContentResponse(contentTypeId);
    }

    private async Task<DocumentResponse> HandleV4LocaleAlreadyUsedAsync(
        string singularContentTypeId,
        string pluralContentTypeId,
        string contentId,
        string jsonContent,
        string targetLanguage)
    {
        var response = await GetLocalizationObjectsV4Async(new()
        {
            ContentTypeId = singularContentTypeId,
            ContentId = contentId,
            StrapiVersion = StrapiVersions.V4
        });

        var locales = MissingLocalesResponse.GetIdsWithLocalesFromJObject(response, singularContentTypeId);
        var existingLocale = locales.FirstOrDefault(l => l.Locale == targetLanguage);
        if (existingLocale != null)
        {
            var body = BuildV4UpdateRequestBody(jsonContent);
            var apiRequest = new RestRequest($"/api/{pluralContentTypeId}/{existingLocale.Id}", Method.Put)
                .AddStringBody(body, ContentType.Json);

            var jObject = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
            return jObject.ToFullContentResponse(pluralContentTypeId);
        }
        else
        {
            throw new PluginApplicationException("Failed to find existing localization, although Strapi returned that the locale is already used. Please, ask Blackbird support for further investigation");
        }
    }
    
    private static string BuildV4UpdateRequestBody(string rawJson)
    {
        var jsonObject = JObject.Parse(rawJson);
        var propertiesToRemove = jsonObject.Properties()
            .Where(p => p.Value.Type == JTokenType.Null)
            .ToList();

        foreach (var prop in propertiesToRemove)
        {
            prop.Remove();
        }

        var payload = new JObject(new JProperty("data", jsonObject));
        return JsonConvert.SerializeObject(payload);
    }

    private async Task<DocumentWithLocalizationsResponse> GetLocalizationObjectsV5Async(GetMissingLocalesRequest request)
    {
        try
        {
            var apiRequest = new RestRequest($"/api/{request.ContentTypeId}/{request.ContentId}")
                .AddQueryParameter("populate", "localizations");
            var response = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
            return response.ToContentWithLocalizationsResponse(request.ContentTypeId);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException($"Error: {ex.Message}. If you're using Strapi v4, specify this in the 'Strapi version' field", ex);
        }
    }

    private async Task<JObject> GetLocalizationObjectsV4Async(GetMissingLocalesRequest identifier)
    {
        try
        {
            var query = GraphQlQueries.GetLocalizationObjectsForContentQuery(identifier.ContentTypeId, identifier.ContentId!);
            var graphQlRequest = new RestRequest("/graphql", Method.Post)
                .AddJsonBody(new
                {
                    query
                });

            return await Client.ExecuteWithErrorHandling<JObject>(graphQlRequest);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException("Failed to retrieve localization objects through the GraphQL API. Please, verify that graphQL plugin is installed and enabled in your Strapi instance.", ex);
        }
    }

    private async Task<List<LanguageDto>> GetAllAvailableLanguagesAsync()
    {
        var apiRequest = new RestRequest("/api/i18n/locales");
        return await Client.ExecuteWithErrorHandling<List<LanguageDto>>(apiRequest);
    }
}