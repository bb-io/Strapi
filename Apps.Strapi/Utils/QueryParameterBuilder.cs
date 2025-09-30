using RestSharp;

namespace Apps.Strapi.Utils;

public static class QueryParameterBuilder
{
    public static void AddFieldFiltersIfAvailable(
        RestRequest request, 
        IEnumerable<string>? fieldPaths, 
        IEnumerable<string>? fieldValues, 
        string @operator = "$eq")
    {
        if (fieldPaths == null || fieldValues == null)
        {
            return;
        }
            
        var keyValues = fieldPaths.Zip(fieldValues, (field, value) => new { field, value });
        foreach (var keyValue in keyValues)
        {
            if (string.IsNullOrEmpty(keyValue.field) || string.IsNullOrEmpty(keyValue.value))
            {
                continue;
            }
                
            var lastPart = keyValue.field.Contains(".") 
                ? keyValue.field.Split('.').Last() 
                : keyValue.field;
                
            var queryParameter = $"filters[{lastPart}][{@operator}]";
            var value = keyValue.value.Equals("null", StringComparison.OrdinalIgnoreCase) 
                ? null 
                : keyValue.value;
            
            request.AddQueryParameter(queryParameter, value);
        }
    }
}
