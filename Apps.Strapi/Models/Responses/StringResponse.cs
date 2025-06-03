namespace Apps.Strapi.Models.Responses;

public class StringResponse(string value)
{
    public string Value { get; set; } = value;
}
