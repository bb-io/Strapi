using Apps.Strapi.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Strapi.Handlers;

public class LanguageDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var apiRequest = new RestRequest("/api/i18n/locales");
        var languages = await Client.ExecuteWithErrorHandling<List<LanguageDto>>(apiRequest);
        return languages
            .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.Code, x.Name));
    }
}