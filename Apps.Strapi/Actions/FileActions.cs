using Apps.Strapi.Models.Requests.Files;
using Apps.Strapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Text.Json;

namespace Apps.Strapi.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Get a list of files", Description = "Get a list of files")]
    public async Task<IEnumerable<ApiFileResponse>> GetFiles()
    {
        var result = await Client.ExecuteWithErrorHandling<IEnumerable<ApiFileResponse>>(new RestRequest($"/api/upload/files", Method.Get));
        
        return result;
    }

    [Action("Get a file", Description = "Get a specific file")]
    public async Task<ApiFileResponse> GetFile([ActionParameter] GetFileRequest getFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<ApiFileResponse>(new RestRequest($"/api/upload/files/{getFileRequest.Id}", Method.Get));

        return result;
    }

    [Action("Upload files", Description = "Upload one or more files to your application.")]
    public async Task UploadFiles([ActionParameter] UploadFilesRequest uploadFilesRequest)
    {
        var request = new RestRequest($"/api/upload/", Method.Post);

        foreach (var item in uploadFilesRequest.Files)
        {
            var file = await fileManagementClient.DownloadAsync(item);
            request.AddFile(item.Name, () => { return file; }, item.Name);
        }

        var result = await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Upload files entry", Description = "Upload one or more files that will be linked to a specific entry.")]
    public async Task UploadFiles([ActionParameter] UploadFileEntryRequest uploadFilesEntryRequest)
    {
        var request = new RestRequest($"/api/upload", Method.Post);

        if (!string.IsNullOrEmpty(uploadFilesEntryRequest.Path))
        {
            request.AddParameter("path", uploadFilesEntryRequest.Path);
        }
        if (!string.IsNullOrEmpty(uploadFilesEntryRequest.Source))
        {
            request.AddParameter("source", uploadFilesEntryRequest.Source);
        }

        if (string.IsNullOrEmpty(uploadFilesEntryRequest.RefId) || string.IsNullOrEmpty(uploadFilesEntryRequest.Ref) || string.IsNullOrEmpty(uploadFilesEntryRequest.Field))
        {
            throw new PluginMisconfigurationException("Please make sure to provide Refid, Ref and Field parameters");
        }

        request.AddParameter("refId", uploadFilesEntryRequest.RefId);
        request.AddParameter("ref", uploadFilesEntryRequest.Ref);
        request.AddParameter("field", uploadFilesEntryRequest.Field);

        foreach (var item in uploadFilesEntryRequest.Files)
        {
            var file = await fileManagementClient.DownloadAsync(item);
            request.AddFile(item.Name, () => { return file; }, item.Name);
        }
        var result = await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Update file info", Description = "Update a file in your application.")]
    public async Task UpdateFileInfo([ActionParameter] UpdateFileInfoRequest updateFileInfoRequest)
    {
        var request = new RestRequest($"/api/upload?id={updateFileInfoRequest.Id}", Method.Post);

        request.AddParameter("fileInfo",JsonSerializer.Serialize(updateFileInfoRequest.FileInfo));

        var result = await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Delete a file", Description = "Delete a file from your application.")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest deleteFileRequest)
    {
        var result = await Client.ExecuteWithErrorHandling<ApiFileResponse>(new RestRequest($"/api/upload/files/{deleteFileRequest.Id}", Method.Delete));
    }
}
