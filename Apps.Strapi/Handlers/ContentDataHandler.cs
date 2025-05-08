using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Strapi.Handlers;

public class ContentDataHandler(InvocationContext invocationContext, [ActionParameter] ContentTypeIdentifier identifier) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(identifier.ContentTypeId))
        {
            throw new Exception("You must provide a Content type ID first to fetch the content IDs.");
        }

        var apiRequest = new RestRequest($"/api/{identifier.ContentTypeId}");
        var result = await Client.PaginateAsync<JObject>(apiRequest);
        
        var searchString = context.SearchString;
        return result
            .Where(x => x["documentId"]?.ToString() != null)
            .Select(x => {
                string id = x["documentId"]!.ToString();
                return new DataSourceItem(id, GetContentName(x, id));
            })
            .Where(x => searchString == null || 
                  x.DisplayName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
    }

    private string GetContentName(JObject content, string defaultValue)
    {
        foreach (var property in content.Properties())
        {
            if (property.Name.Equals("name", StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                return property.Value.ToString() ?? defaultValue;
            }
        }

        return defaultValue;
    }
}