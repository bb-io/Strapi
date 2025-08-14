using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Strapi.Handlers;

public class ContentDataHandler(InvocationContext invocationContext, [ActionParameter] ContentTypeIdentifier identifier) 
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier.ContentTypeId))
        {
            throw new Exception("You must provide a Content type ID first to fetch the content IDs.");
        }

        var apiRequest = new RestRequest($"/api/{identifier.ContentTypeId}");
        var result = await Client.PaginateAsync<JObject>(apiRequest);
        var documents = result.ToContentListResponse(identifier.ContentTypeId);

        return documents
            .Where(x => x.DocumentId != null && x.Title != null)
            .Where(x => context.SearchString == null || x.Title!.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.DocumentId!, x.Title!));
    }
}