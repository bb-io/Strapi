using Apps.Strapi.Constants;
using Apps.Strapi.Handlers;
using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using RestSharp;

namespace Apps.Strapi.Models.Requests;

public class SearchContentRequest : VersionOptionalRequest
{
    [Display("Content type IDs", Description = "The content types you want to search for.")]
    public IEnumerable<string> ContentTypeIds { get; set; } = [];

    [Display("Language", Description = "The language of the content"), DataSource(typeof(SearchLanguageDataHandler))]
    public string? Language { get; set; }

    [Display("Status", Description = "The status of the content"), StaticDataSource(typeof(StatusDataHandler))]
    public string? Status { get; set; }

    [Display("Field names", Description = "The names of the fields to filter the content by.")]
    public IEnumerable<string>? FieldNames { get; set; }

    [Display("Field values", Description = "The values of the fields to filter the content by. Add 'null' to filter by empty values.")]
    public IEnumerable<string>? FieldValues { get; set; }

    [Display("Updated after", Description = "Return only documents updated strictly after this timestamp (UTC).")]
    public DateTime? UpdatedAfter { get; set; }
    
    [Display("Created after", Description = "Return only documents created strictly after this timestamp (UTC).")]
    public DateTime? CreatedAfter { get; set; }

    public void ApplyStatusIfPresent(RestRequest restRequest)
    {
        var version = GetVersionOrDefault();
        if (version == StrapiVersions.V5)
        {
            if(!string.IsNullOrEmpty(Status))
            {
                restRequest.AddQueryParameter("status", Status);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Status) && Status == "draft")
            {
                restRequest.AddQueryParameter("publicationState", "preview");
                restRequest.AddQueryParameter("filters[publishedAt][$null]", "true");
            }
            else if (!string.IsNullOrEmpty(Status) && Status == "published")
            {
                restRequest.AddQueryParameter("publicationState", "live");
                restRequest.AddQueryParameter("filters[publishedAt][$null]", "false");
            }
        }
    }
}