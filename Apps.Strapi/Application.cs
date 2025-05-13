using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Strapi;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [
            ApplicationCategory.Cms
        ];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
