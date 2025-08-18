using Apps.Strapi.Constants;
using Apps.Strapi.Models.Dtos;
using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Apps.Strapi.Models.Responses;
using Apps.Strapi.Utils;
using Apps.Strapi.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Models.Responses;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Strapi.Actions;

[ActionList]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search content", Description = "Returns a list of content based on specified inputs. Only for collection types content types.")]
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

            if (request.Status != null)
            {
                apiRequest.AddQueryParameter("status", request.Status);
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

        if (request.ContentTypeId.EndsWith("s"))
        {
            request.ContentTypeId = request.ContentTypeId[..^1];
        }

        var languages = await GetAllAvailableLanguagesAsync();
        var targetLocales = languages.Select(l => l.Code).ToList();
        if (request.StrapiVersion == StrapiVersions.V4)
        {
            var response = await GetLocalizationObjectsV4Async(request, request.StrapiVersion);
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
        var htmlString = JsonToHtmlConverter.ConvertToHtml(response.Content!, identifier.ContentId, identifier.ContentTypeId, downloadContentRequest.ExcludeFields);
        var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlString))
        {
            Position = 0
        };

        var title = JsonToHtmlConverter.ExtractTitle(response.Content!, identifier.ContentId ?? identifier.ContentTypeId);
        var fileReference = await fileManagementClient.UploadAsync(memoryStream, "text/html", $"{title}.html");

        return new(fileReference, identifier.ContentTypeId);
    }

    [Action("Upload content", Description = "Uploads a HTML file to a specific language to localize the content.")]
    public async Task<DocumentResponse> UploadContentAsync([ActionParameter] UploadContentRequest request)
    {
        var strapiVersion = request.StrapiVersion ?? "v5";
        if (!StrapiVersions.AllVersions.Contains(strapiVersion))
        {
            var supportedVersions = string.Join(", ", StrapiVersions.AllVersions);
            throw new PluginMisconfigurationException($"Unsupported Strapi version '{strapiVersion}'. Supported versions are: {supportedVersions}.");
        }

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var htmlString = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        var metadata = HtmlToJsonConverter.ExtractMetadata(htmlString);
        var jsonContent = HtmlToJsonConverter.ConvertToJson(htmlString, strapiVersion, request.TargetLanguage);

        var endpoint = $"/api/{metadata.ContentTypeId}";
        if (!string.IsNullOrEmpty(metadata.ContentId))
        {
            endpoint += $"/{metadata.ContentId}";
        }

        if (StrapiVersions.V5 == strapiVersion)
        {
            var apiRequest = new RestRequest(endpoint, Method.Put)
                .AddQueryParameter("locale", request.TargetLanguage)
                .AddBody(jsonContent, ContentType.Json);

            var jObject = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
            return jObject.ToFullContentResponse(metadata.ContentTypeId);
        }
        else if (StrapiVersions.V4 == strapiVersion)
        {
            endpoint += "/localizations";
            var apiRequest = new RestRequest(endpoint, Method.Post)
                .AddStringBody(jsonContent, ContentType.Json);

            var jObject = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);
            return jObject.ToContentResponse(metadata.ContentTypeId);
        }
        else
        {
            throw new PluginMisconfigurationException($"Operation not supported yet for Strapi version '{strapiVersion}'. Ask blackbird support to implement it.");
        }
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

    private async Task<JObject> GetLocalizationObjectsV4Async(GetMissingLocalesRequest identifier, string locale)
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