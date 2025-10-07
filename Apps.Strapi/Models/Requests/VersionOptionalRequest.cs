using Apps.Strapi.Constants;
using Apps.Strapi.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Strapi.Models.Requests;

public class VersionOptionalRequest
{
    [Display("Strapi version", Description = "The version of Strapi to use for the upload. By default we will use version 5."), StaticDataSource(typeof(StrapiVersionDataHandler))]
    public string? StrapiVersion { get; set; }
    
    public string GetVersionOrDefault() => StrapiVersion ?? StrapiVersions.V5;
}
