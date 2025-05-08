namespace Apps.Strapi.Models.Responses
{
    public class FilesResponse
    {
        IEnumerable<ApiFileResponse> Files { get; set; } = new List<ApiFileResponse>();
    }
}
