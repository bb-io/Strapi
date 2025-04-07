using System.Text.Json.Serialization;

namespace Apps.Strapi.Models.Responses
{
    public class FileResponse
    {
        [JsonPropertyName("id")]
        int Id { get; set; }

        [JsonPropertyName("documentId")]
        string DocumentId { get; set; }

        [JsonPropertyName("name")]
        string Name { get; set; }

        //string? AlternativeText { get; set; }

        //string? Caption { get; set; }

        //string? Width { get; set; }

        //string? Height { get; set; }

        //string? Formats { get; set; }

        //string? hash { get; set; }

        //string? ext { get; set; }

        //string? mime { get; set; }

        //float? size { get; set; }

        //string? url { get; set; }

        //string? previewUrl { get; set; }

        //string? provider { get; set; }


        //string? provider_metadata { get; set; }

        //DateTime? createdAt { get; set; }

        //DateTime? updatedAt { get; set; }

        //string? publishedAt { get; set; }

    }
}