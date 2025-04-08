namespace Models.Responses
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public string? DocumentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public string? PublishedAt { get; set; }
        public string? Locale { get; set; }
    }
}
