using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Apps.Strapi.Models.Responses;
using Apps.Strapi.Utils;
using Apps.Strapi.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
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
        var apiRequest = new RestRequest($"/api/{request.ContentTypeId}");
        if(request.Language != null)
        {
            apiRequest.AddQueryParameter("locale", request.Language);
        }

        if(request.Status != null)
        {
            apiRequest.AddQueryParameter("status", request.Status);
        }

        var result = await Client.PaginateAsync<JObject>(apiRequest);
        return new(result.ToContentListResponse());
    }

    [Action("Download content", Description = "Downloads a content by ID. By default  it will download the content for published status")]
    public async Task<FileResponse> DownloadContentAsync([ActionParameter] ContentLanguageIdentifier identifier,
        [ActionParameter] ContentStatusOptionalRequest optionalRequest,
        [ActionParameter] DownloadContentRequest downloadContentRequest)
    {
        ExceptionExtensions.ThrowIfNullOrEmpty(identifier.ContentTypeId, "Content type ID");

        var request = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}");
        if(identifier.Language != null)
        {
            request.AddQueryParameter("locale", identifier.Language);
        }

        if(optionalRequest.Status != null)
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

        return new(fileReference);
    }    
    
    [Action("Upload content", Description = "Uploads a HTML file to a specific language to localize the content.")]
    public async Task UploadContentAsync([ActionParameter] UploadContentRequest request)
    {
        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var htmlString = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        var metadata = HtmlToJsonConverter.ExtractMetadata(htmlString);
        var jsonContent = HtmlToJsonConverter.ConvertToJson(htmlString);

        var endpoint = $"/api/{metadata.ContentTypeId}";
        if(!string.IsNullOrEmpty(metadata.ContentId))
        {
            endpoint += $"/{metadata.ContentId}";
        }

        var apiRequest = new RestRequest(endpoint, Method.Put)
            .AddQueryParameter("locale", request.TargetLanguage)
            .AddBody(jsonContent, ContentType.Json);

        await Client.ExecuteWithErrorHandling(apiRequest);
    }

    [Action("Delete content", Description = "Deletes a content by ID.")]
    public async Task DeleteContentAsync([ActionParameter] ContentIdentifier request)
    {
        var apiRequest = new RestRequest($"/api/{request.ContentTypeId}/{request.ContentId}", Method.Delete);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}