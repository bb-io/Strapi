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
    [Action("Get a list of files", Description = "Get a list of files")]
    public async Task<IEnumerable<FileResponse>> GetFiles()
    {
        var result = await Client.ExecuteWithErrorHandling<IEnumerable<FileResponse>>(new RestRequest($"/api/upload/files", Method.Get));
        
        return result;
    }

    [Action("Get a file", Description = "Get a specific file")]
    public async Task<FileResponse> GetFile([ActionParameter] GetFileRequest getFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload/files/{getFileRequest.Id}", Method.Get));

        return result;
    }

    //TODO: create separete upload entry files???
    [Action("Upload files", Description = "Upload one or more files to your application.")]
    public async Task UploadFiles([ActionParameter] UploadFilesRequest uploadFilesRequest) //TODO: fill upload body with request parameters
    {
        var result = await Client.ExecuteWithErrorHandling(new RestRequest($"/api/upload/", Method.Post));
    }

    [Action("Upload file info", Description = "Update a file in your application.")]
    public async Task UploadFileInfo([ActionParameter] UploadFileInfoRequest uploadFileInfoRequest)
    {
        //TODO: send file info in body
        var result = await Client.ExecuteWithErrorHandling(new RestRequest($"/api/upload?id={uploadFileInfoRequest.Id}", Method.Post));
    }

    [Action("Delete a file", Description = "Delete a file from your application.")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest deleteFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<FileResponse>(new RestRequest($"/api/upload/files/{deleteFileRequest.Id}", Method.Delete));
    }
}
