using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
namespace Apps.Strapi.Handlers.Static
{
    internal class StatusDataHandler : IStaticDataSourceItemHandler
    {
        public IEnumerable<DataSourceItem> GetData()
        {
            var items = new List<DataSourceItem>
            {
                new DataSourceItem("draft", "Draft"),
                new DataSourceItem("published", "Published")
            };

            return items;
        }
    }
}
