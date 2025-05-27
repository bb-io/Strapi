using Apps.Strapi.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Strapi.Handlers.Static;

public class StrapiVersionDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new(StrapiVersions.V4, "Strapi v4"),
            new(StrapiVersions.V5, "Strapi v5"),
        ];
    }
}