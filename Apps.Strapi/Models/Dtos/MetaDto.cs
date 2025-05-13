namespace Apps.Strapi.Models.Dtos;

public class MetaDto
{
    public PaginationModelDto Pagination { get; set; } = new();
}

public class PaginationModelDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public int Total { get; set; }
}