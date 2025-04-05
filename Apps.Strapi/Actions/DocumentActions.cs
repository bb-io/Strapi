using Apps.Strapi.Models.Requests.Documents;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Models.Responses;
using RestSharp;

namespace Apps.Strapi.Actions;

[ActionList]
public class DocumentActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get Documents", Description = "Gets a list of document.")] //TODO: fill description  // TODO: create filters
    public async Task<DocumentsResponse> GetDocuments([ActionParameter]GetDocumentsRequest request)
    {
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest($"/{request.ApiId}", Method.Get));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
        return new()
        {
            Data = result.Data
        };
    }

    [Action("Get Document", Description = "Gets a document.")] //TODO: fill description
    public async Task<DocumentsResponse> GetDocument([ActionParameter] GetDocumentRequest request) // TODO: create filters
    {
        string query = string.Empty;
        if (request.Id != null)
        {
            query = $"/{request.ApiId}/{request.Id}";
        }
        else
        {
            query = $"/{request.ApiId}";
        }
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest(query, Method.Get));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
        throw new Exception();
    }
    [Action("Create Document", Description = "Creates a new document")]//TODO: fill description
    public async Task<DocumentResponse> CreateDocument([ActionParameter] CreateDocumentRequest request)
    {
        string query = string.Empty;
        if (request.Id != null)
        {
            query = $"/{request.ApiId}/{request.Id}";
        }
        else
        {
            query = $"/{request.ApiId}"; // TODO: this needs to be a put
        }
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest(query, Method.Post));

        if (result == null)
        {
            throw new PluginApplicationException();
        }

        return new DocumentResponse
        {
            Id = result.Data.Id,
            DocumentId = result.Data.DocumentId,
            Title = result.Data.Title,
            Description = result.Data.Description,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            PublishedAt = result.Data.PublishedAt,
            Locale = result.Data.Locale
        };
    }

    [Action("Update Document", Description = "Updates an existing document")]
    public async Task<DocumentResponse> UpdateDocument([ActionParameter] UpdateDocumentRequest request) // TODO: create filters
    {
        string query = string.Empty;
        if (request.Id != null)
        {
            query = $"/{request.ApiId}/{request.Id}";
        }
        else
        {
            query = $"/{request.ApiId}";
        }
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest(query, Method.Put));

        if (result == null)
        {
            throw new PluginApplicationException();
        }

        return new DocumentResponse
        {
            Id = result.Data.Id,
            DocumentId = result.Data.DocumentId,
            Title = result.Data.Title,
            Description = result.Data.Description,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            PublishedAt = result.Data.PublishedAt,
            Locale = result.Data.Locale
        };
    }

    [Action("Delete Document", Description = "Deletes a document")]
    public async Task DeleteDocument([ActionParameter] DeleteDocumentRequest request)
    {
        string query = string.Empty;
        if (request.Id != null)
        {
            query = $"/{request.ApiId}/{request.Id}";
        }
        else
        {
            query = $"/{request.ApiId}";
        }
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest(query, Method.Delete));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
    }
}
