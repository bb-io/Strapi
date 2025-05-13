namespace Apps.Strapi.Models.Dtos;

public class PaginationDto<T>
{
    public List<T> Data { get; set; } = new();
    public MetaDto Meta { get; set; } = new();
}