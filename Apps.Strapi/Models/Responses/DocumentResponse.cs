namespace Models.Responses
{
    public class DocumentResponse
    {
        public int Id { get; set; } // TODO move to the base class
        public string? DocumentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? CreatedAt { get; set;}
        public string? UpdatedAt { get; set; }
        public string? PublishedAt { get; set; }
        public string? Locale { get; set; }
        //        "id": 2,
        //"documentId": "hgv1vny5cebq2l3czil1rpb3",
        //"Name": "BMK Paris Bamako",
        //"Description": null,
        //"createdAt": "2024-03-06T13:42:05.098Z",
        //"updatedAt": "2024-03-06T13:42:05.098Z",
        //"publishedAt": "2024-03-06T13:42:05.103Z",
        //"locale": "en"
    }
}
