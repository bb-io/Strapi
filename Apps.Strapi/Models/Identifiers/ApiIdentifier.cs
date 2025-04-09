using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Identifiers
{
    public class ApiIdentifier
    {
        [Display("Api DocumentId", Description = "Description")]
        public string ApiId { get; set; }
    }
}
