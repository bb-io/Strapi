using Apps.Strapi.Models.Requests.Files;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Text.Json.Nodes;

namespace Apps.Strapi.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get a list of files", Description = "Action description")] //TODO: fill description
    public async Task<IEnumerable<FileResponse>> GetFiles()
    {
        var result = await Client.ExecuteWithErrorHandling<IEnumerable<FileResponse>>(new RestRequest($"/api/upload/files", Method.Get));
        return result;
    }

    [Action("Get a file", Description = "Action description")] //TODO: fill description
    public async Task<FileResponse> GetFile([ActionParameter] GetFileRequest getFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload/files/{getFileRequest.Id}", Method.Get));

        return result;
    }

    [Action("Upload files", Description = "Action description")] //TODO: fill description
    public async Task UploadFiles([ActionParameter] UploadFilesRequest uploadFilesRequest)
    {
        //TODO fill upload methods
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload/", Method.Post));
    }

    [Action("Upload file info", Description = "Action description")] //TODO: fill description
    public async Task UploadFileInfo([ActionParameter] UploadFileInfoRequest uploadFileInfoRequest)
    {
        //TODO: fill process
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload?id={uploadFileInfoRequest.Id}", Method.Get));
    }

    [Action("Delete a file", Description = "Action description")] //TODO: fill description
    public async Task DeleteFile([ActionParameter] DeleteFileRequest deleteFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload/files/{deleteFileRequest.Id}", Method.Delete));
    }
}
