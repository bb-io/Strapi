using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Apps.Strapi.Models.Responses
{
    public class FileResponse
    {
        [JsonProperty("id")]
        int Id { get; set; }

        [JsonProperty("DocumentId")]
        string DocumentId { get; set; }

        [JsonProperty("name")]
        string Name { get; set; }

        [JsonProperty("alternativeText")]
        string? AlternativeText { get; set; }

        [JsonProperty("caption")]
        string? Caption { get; set; }

        [JsonProperty("width")]
        string? Width { get; set; }

        [JsonProperty("height")]
        string? Height { get; set; }
        [JsonProperty("formats")]
        string? Formats { get; set; }
        [JsonProperty("hash")]
        string? Hash { get; set; }

        [JsonProperty("ext")]
        string? Ext { get; set; }

        [JsonProperty("mime")]
        string? Mime { get; set; }

        [JsonProperty("size")]
        float? Size { get; set; }

        [JsonProperty("url")]
        string? Url { get; set; }

        [JsonProperty("previewUrl")]
        string? PreviewUrl { get; set; }

        [JsonProperty("provider")]
        string? Provider { get; set; }

        [JsonProperty("provider_metadata")]
        string? ProviderMetadata { get; set; }

        [JsonProperty("createdAt")]
        DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        DateTime? UpdatedAt { get; set; }

        [JsonProperty("publishedAt")]
        string? PublishedAt { get; set; }

    }
}