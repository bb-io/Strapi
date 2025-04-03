using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Strapi.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get a list of files", Description = "Action description")] //TODO: fill description
    public async Task<IEnumerable<FileResponse>> GetFiles()
    {
        throw new NotImplementedException();
    }

    [Action("Get a file", Description = "Action description")] //TODO: fill description
    public async Task<FileResponse> GetFile()
    {
        throw new NotImplementedException();
    }

    [Action("Upload a file", Description = "Action description")] //TODO: fill description
    public async Task UploadFile()
    {
        throw new NotImplementedException();
    }

    [Action("Upload file info", Description = "Action description")] //TODO: fill description
    public async Task UploadFileInfo()
    {
        throw new NotImplementedException();
    }

    [Action("Delete a file", Description = "Action description")] //TODO: fill description
    public async Task DeleteFile()
    {
        throw new NotImplementedException();
    }
}
