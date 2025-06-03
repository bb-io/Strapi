namespace Apps.Strapi.Models.Dtos;

public class LanguageDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}