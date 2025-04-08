using Models.Responses;

namespace Apps.Strapi.Models.Responses
{
    public class DocumentsResponse
    {
        public IEnumerable<DocumentResponse> Data { get; set; }

        //public string? Meta { get; set; }
    }
}
