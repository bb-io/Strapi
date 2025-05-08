using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Strapi.Base;

public abstract class BaseDataHandlerTests : TestBase
{
    protected abstract IAsyncDataSourceItemHandler DataHandler { get; }
    protected abstract string SearchString { get; }
    protected virtual bool CanBeEmpty => false; 

    [TestMethod]
    public virtual async Task GetDataAsync_WithoutSearchString_ShouldReturnNonEmptyCollection()
    {
        await TestDataHandlerAsync(DataHandler);
    }

    [TestMethod]
    public virtual async Task GetDataAsync_WithSearchString_ShouldReturnNonEmptyCollection()
    {
        await TestDataHandlerAsync(DataHandler, SearchString);
    }

    private async Task TestDataHandlerAsync(IAsyncDataSourceItemHandler dataHandler, string? searchString = null)
    {
        var context = new DataSourceContext { SearchString = searchString };
        var result = await dataHandler.GetDataAsync(context, CancellationToken.None);

        Assert.IsNotNull(result);
        if(CanBeEmpty == false)
        {
            Assert.IsTrue(result.Any(), "Result should not be empty.");
        }

        Assert.IsTrue(result.All(item => !string.IsNullOrEmpty(item.DisplayName)), "All items should have a name.");
        if (!string.IsNullOrEmpty(searchString))
        {
            Assert.IsTrue(result.All(item => item.DisplayName.Contains(searchString, StringComparison.OrdinalIgnoreCase)), 
                $"All items should contain the search string '{searchString}'.");
        }

        LogItems(result);
    }

    private static void LogItems(IEnumerable<DataSourceItem> items)
    {
        Console.WriteLine($"Total items: {items.Count()}");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Value}, Name: {item.DisplayName}");
        }
    }
}