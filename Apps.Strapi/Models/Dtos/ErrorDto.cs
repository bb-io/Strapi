using Newtonsoft.Json;

namespace Apps.Strapi.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("error")]
    public ErrorDetailsDto Error { get; set; } = new();

    override public string ToString()
    {
        return $"{Error.Name}; Message: {Error.Message}, Status: {Error.Status}";
    }
}

public class ErrorDetailsDto
{
    [JsonProperty("status")]
    public int Status { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}