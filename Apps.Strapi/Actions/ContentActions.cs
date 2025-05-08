using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Apps.Strapi.Models.Requests.Documents;
using Apps.Strapi.Models.Responses;
using Apps.Strapi.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Models.Responses;
using RestSharp;

namespace Apps.Strapi.Actions;

[ActionList]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search content", Description = "Returns a list of content based on specified inputs. Only for collection types content types.")]
    public async Task<SearchContentResponse> SearchContentAsync([ActionParameter] SearchContentRequest request)
    {
        var apiRequest = new RestRequest($"/api/{request.ContentTypeId}");
        if(request.Language != null)
        {
            apiRequest.AddQueryParameter("locale", request.Language);
        }

        if(request.Status != null)
        {
            apiRequest.AddQueryParameter("status", request.Status);
        }

        var result = await Client.PaginateAsync<DocumentResponse>(apiRequest);
        return new(result);
    }

    [Action("Download content", Description = "Downloads a content by ID.")]
    public async Task<FileResponse> DownloadContentAsync([ActionParameter] ContentLanguageIdentifier identifier)
    {
        var request = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}");
        if(identifier.Language != null)
        {
            request.AddQueryParameter("locale", identifier.Language);
        }

        var response = await Client.ExecuteWithErrorHandling(request);       
        var htmlString = JsonToHtmlConverter.ConvertToHtml(response.Content!, identifier.ContentId);
        var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlString));
        memoryStream.Position = 0;

        var title = JsonToHtmlConverter.ExtractTitle(response.Content!, identifier.ContentId);
        var fileReference = await fileManagementClient.UploadAsync(memoryStream, "text/html", $"{title}.html");

        return new(fileReference);
    }

    [Action("Update Document", Description = "Partially updates a document by id and returns its value.")]
    public async Task<DocumentResponse> UpdateDocument([ActionParameter] UpdateDocumentRequest request)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(request.DocumentId))
        {
            query = $"/{request.ApiId}/{request.DocumentId}";
        }
        else
        {
            query = $"/{request.ApiId}";
        }
        var restRequest = new RestRequest(query, Method.Put);
        var file = await fileManagementClient.DownloadAsync(request.File);
        using var sr = new StreamReader(file);
        var fileContent = sr.ReadToEnd();

        restRequest.AddBody(fileContent, "application/json");

        var result = await Client.ExecuteWithErrorHandling<DocumentResponse>(restRequest);

        if (result == null)
        {
            throw new PluginApplicationException();
        }

        return new DocumentResponse
        {
            DocumentId = result.DocumentId,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            PublishedAt = result.PublishedAt,
            Locale = result.Locale
        };
    }

    [Action("Delete content", Description = "Deletes a content by ID.")]
    public async Task DeleteContentAsync([ActionParameter] ContentIdentifier request)
    {
        var apiRequest = new RestRequest($"/api/{request.ContentTypeId}/{request.ContentId}", Method.Delete);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}