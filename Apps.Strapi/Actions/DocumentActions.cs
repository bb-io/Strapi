using Apps.Strapi.Api;
using Apps.Strapi.Models.Requests;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Models.Requests;
using Models.Responses;
using RestSharp;

namespace Apps.Strapi.Actions;

[ActionList]
public class DocumentActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get Documents", Description = "Gets a list of document.")] //TODO: fill description
    public async Task<DocumentsResponse> GetDocuments([ActionParameter]GetDocumentRequest request)
    {

        string query = string.Empty;
        if (request.Id!=null)
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
        return new()
        {
            Data = result.Data
        };
    }

    [Action("Get Document", Description = "Gets a document.")] //TODO: fill description
    public async Task<DocumentsResponse> GetDocument([ActionParameter] GetDocumentRequest request)
    {
        var result = await Client.GetAsync<DocumentsResponse>(new RestRequest($"/:{request.Id}", Method.Get));

        if (result == null)
        {
            throw new PluginApplicationException();
        }
        throw new Exception();
    }
    [Action("Create Document", Description = "Creates a new document")]//TODO: fill description
    public async Task<DocumentResponse> CreateDocument([ActionParameter] CreateDocumentRequest request)
    {
        throw new NotImplementedException();
    }

    [Action("Update Document", Description = "Updates an existing document")]
    public async Task<DocumentResponse> UpdateDocument([ActionParameter] UpdateDocumentRequest request)
    {
        throw new NotImplementedException();
    }

    [Action("Delete Document", Description = "Deletes a document")]
    public async Task DeleteDocument([ActionParameter] DeleteDocumentRequest request)
    {
        throw new NotImplementedException();
    }
}
