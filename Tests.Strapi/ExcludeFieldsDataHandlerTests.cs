using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class ExcludeFieldsDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new ExcludeFieldsDataHandler(InvocationContext, new()
    {
        ContentTypeId = "animals",
        ContentId = "10"
    });

    protected override string SearchString => "Lion";
}