namespace Apps.Strapi.Models.Responses
{
    public class FilesResponse
    {
        IEnumerable<FileResponse> Files { get; set; } = new List<FileResponse>();
    }
}
