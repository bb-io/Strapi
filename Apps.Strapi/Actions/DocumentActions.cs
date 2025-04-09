using Apps.Strapi.Models.Requests.Documents;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Models.Responses;
using RestSharp;

namespace Apps.Strapi.Actions;

[ActionList]
public class DocumentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext, fileManagementClient)
{
    [Action("Get Documents", Description = "Returns documents matching the query filters")]
    public async Task<DocumentsResponse> GetDocuments([ActionParameter]GetDocumentsRequest request, [ActionParameter] ParametersRequest? parametersRequest = default)
    {
        string filter = BuildParameters(parametersRequest);

        var result = await Client.ExecuteWithErrorHandling<DocumentsResponse>(new RestRequest($"{request.ApiId}{filter}", Method.Get));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
        return new()
        {
            Data = result.Data
        };
    }



    [Action("Get Document", Description = "Returns a document by DocumentId.")]
    public async Task<DocumentResponse> GetDocument([ActionParameter] GetDocumentRequest request, [ActionParameter] ParametersRequest parametersRequest = default)
    {
        string query = string.Empty;
        var filter = BuildParameters(parametersRequest);

        if (!string.IsNullOrEmpty(request.DocumentId))
        {
            query = $"/{request.ApiId}/{request.DocumentId}{filter}";
        }
        else
        {
            query = $"/{request.ApiId}{filter}";
        }
        var result = await Client.ExecuteWithErrorHandling<DocumentResponse>(new RestRequest(query, Method.Get));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
        return result;
    }
    [Action("Create Document", Description = "Creates a document and returns its value.")]
    public async Task<DocumentResponse> CreateDocument([ActionParameter] CreateDocumentRequest request)
    {
        string query = string.Empty;
        Method method;
        if (!string.IsNullOrEmpty(request.DocumentId))
        {
            query = $"/{request.ApiId}/{request.DocumentId}";
            method = Method.Post;
        }
        else
        {
            query = $"/{request.ApiId}";
            method = Method.Post;
        }
        var restRequest = new RestRequest(query, method);

        var file = await FileManagementClient.DownloadAsync(request.File);

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
            Id = result.Id,
            DocumentId = result.DocumentId,
            Title = result.Title,
            Description = result.Description,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            PublishedAt = result.PublishedAt,
            Locale = result.Locale
        };
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
        var file = await FileManagementClient.DownloadAsync(request.File);
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
            Id = result.Id,
            DocumentId = result.DocumentId,
            Title = result.Title,
            Description = result.Description,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            PublishedAt = result.PublishedAt,
            Locale = result.Locale
        };
    }

    [Action("Delete Document", Description = "Deletes a document.")]
    public async Task DeleteDocument([ActionParameter] DeleteDocumentRequest request)
    {
        string query = string.Empty;
        if (request.DocumentId != null)
        {
            query = $"/{request.ApiId}/{request.DocumentId}";
        }
        else
        {
            query = $"/{request.ApiId}";
        }
        var result = await Client.ExecuteWithErrorHandling(new RestRequest(query, Method.Delete));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
    }

    private static string BuildParameters(ParametersRequest? parametersRequest)
    {
        var parameters = new List<string>();


        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.Filters) ? parametersRequest?.Filters : string.Empty); //TODO make sure it's user friendly. https://docs.strapi.io/cms/api/rest/filters#example-find-users-having-john-as-a-first-name

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.Locale) ? ("locale=" + parametersRequest?.Locale) : string.Empty);

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.Status) ? ("status=" + parametersRequest?.Status) : string.Empty);

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.PopulateFields) ? ("populate=" + parametersRequest?.PopulateFields) : string.Empty); //TODO: do multiple fields https://docs.strapi.io/cms/api/rest/populate-select

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.PopulateFields) ? ("fields=" + parametersRequest?.PopulateFields) : string.Empty); //TODO: do multiple fields 

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.Sort) ? ("sort=" + parametersRequest?.Sort) : string.Empty); //TODO: do multiple fields

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.Page.ToString()) ? ("pagination[" + parametersRequest?.Page + "]") : string.Empty); //TODO: do multiple fields

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.PageSize.ToString()) ? ("pagination[" + parametersRequest?.PageSize + "]") : string.Empty); //TODO: do multiple fields

        parameters.Add(!string.IsNullOrEmpty(parametersRequest?.WithCount.ToString()) ? ("pagination[" + parametersRequest?.WithCount + "]") : string.Empty); //TODO: do multiple fields 

        var query = "?";
        foreach (var item in parameters)
        {
            if (!string.IsNullOrEmpty(item))
            {
                query = query + item+"&";
            }
        }
        query = query.TrimEnd('&');

        return query;
    }
}
