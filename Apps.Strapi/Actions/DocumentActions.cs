using Apps.Strapi.Models.Requests;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Models.Requests;
using Models.Responses;

namespace Apps.Strapi.Actions;

[ActionList]
public class DocumentActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get Documents", Description = "Gets a list of document.")] //TODO: fill description
    public async Task<IEnumerable<DocumentResponse>> GetDocuments(GetDocumentRequest request)
    {
        throw new NotImplementedException();
    }

    [Action("Create Document", Description = "Creates a new document")]//TODO: fill description
    public async Task<DocumentResponse> CreateDocument(CreateDocumentRequest request)
    {
        throw new NotImplementedException();
    }

    [Action("Update Document", Description = "Updates an existing document")]
    public async Task<DocumentResponse> UpdateDocument(UpdateDocumentRequest request)
    {
        throw new NotImplementedException();
    }

    [Action("Delete Document", Description = "Deletes a document")]
    public async Task DeleteDocument(DeleteDocumentRequest request)
    {
        throw new NotImplementedException();
    }
}
