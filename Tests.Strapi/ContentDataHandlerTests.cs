using Apps.Strapi.Handlers;
using Apps.Strapi.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class ContentDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new ContentDataHandler(InvocationContext, new ContentTypeIdentifier
    {
        ContentTypeId = "animals"
    });

    protected override string SearchString => "Lion";
}
