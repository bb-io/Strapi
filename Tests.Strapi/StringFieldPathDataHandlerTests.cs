using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class StringFieldPathDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new StringFieldPathDataHandler(InvocationContext, new()
    {
        ContentTypeId = "animals",
        ContentId = "6"
    });

    protected override string SearchString => "translation";
}