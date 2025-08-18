using Blackbird.Applications.Sdk.Common;

namespace Apps.Strapi.Models.Requests;

public class UpdateContentRequest
{
    [Display("Field names")]
    public IEnumerable<string> FieldNames { get; set; } = [];

    [Display("Field values")]
    public IEnumerable<string> FieldValues { get; set; } = [];
}
