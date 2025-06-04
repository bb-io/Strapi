using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Models.Requests;

public class GetTextFieldValueRequest
{
    [Display("Field path"), DataSource(typeof(StringFieldPathDataHandler))]
    public string FieldPath { get; set; } = string.Empty;
}