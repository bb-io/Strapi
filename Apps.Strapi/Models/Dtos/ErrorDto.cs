using Newtonsoft.Json;

namespace Apps.Strapi.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("error")]
    public ErrorDetailsDto Error { get; set; } = new();

    override public string ToString()
    {
        var baseMessage = $"{Error.Name}; Message: {Error.Message}, Status: {Error.Status}";
        if (Error.Details?.Errors != null && Error.Details.Errors.Any())
        {
            var validationErrors = string.Join("; ", Error.Details.Errors.Select(e => 
                $"Field '{string.Join(".", e.Path)}': {e.Message}"));
            return $"{baseMessage}. Validation errors: {validationErrors}";
        }
        
        return baseMessage;
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
    
    [JsonProperty("details")]
    public ValidationDetailsDto? Details { get; set; }
}

public class ValidationDetailsDto
{
    [JsonProperty("errors")]
    public List<ValidationErrorDto> Errors { get; set; } = new();
}

public class ValidationErrorDto
{
    [JsonProperty("path")]
    public List<string> Path { get; set; } = new();
    
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}