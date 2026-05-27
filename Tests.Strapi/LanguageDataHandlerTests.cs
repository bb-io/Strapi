using Apps.Strapi.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Strapi;

[TestClass]
public class LanguageDataHandlerTests : ContentDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new LanguageDataHandler(InvocationContext);
    
    protected override string SearchString => "en";
}