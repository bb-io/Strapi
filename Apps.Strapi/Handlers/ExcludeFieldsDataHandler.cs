using Apps.Strapi.Constants;
using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Strapi.Handlers;

public class ExcludeFieldsDataHandler(InvocationContext invocationContext, [ActionParameter] ContentIdentifier identifier) 
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier.ContentTypeId))
        {
            throw new Exception("You must provide a Content type ID first to fetch the fields");
        }

        if (string.IsNullOrEmpty(identifier.ContentId))
        {
            throw new Exception("You must provide a Content ID first to fetch the fields");
        }

        var apiRequest = new RestRequest($"/api/{identifier.ContentTypeId}/{identifier.ContentId}");
        var result = await Client.ExecuteWithErrorHandling<JObject>(apiRequest);

        return GetStringPropertiesRecursively(result)
            .Where(property =>
            {
                var lastPart = property.Split('.').Last();
                return !JsonProperties.NonLocalizableProperties.Contains(lastPart);
            })
            .Where(property => context.SearchString == null || property.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .Select(property => new DataSourceItem(property, property))
            .ToList();
    }

    private IEnumerable<string> GetStringPropertiesRecursively(JObject jObject, string prefix = "")
    {
        foreach (var property in jObject.Properties())
        {
            var currentPath = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
            
            if (string.IsNullOrEmpty(prefix) && JsonProperties.NonLocalizableProperties.Contains(property.Name))
                continue;

            // Only include string properties
            if (property.Value.Type == JTokenType.String)
            {
                yield return currentPath;
            }

            if (property.Value is JObject childObject)
            {
                foreach (var childPath in GetStringPropertiesRecursively(childObject, currentPath))
                {
                    yield return childPath;
                }
            }
            else if (property.Value is JArray array)
            {
                foreach (var item in array.OfType<JObject>())
                {
                    foreach (var childPath in GetStringPropertiesRecursively(item, currentPath))
                    {
                        yield return childPath;
                    }
                }
            }
        }
    }
}