using Apps.Strapi.Actions;
using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class ContentActionsTests : TestBase
{
    private ContentActions? _contentActions;

    [TestInitialize]
    public void Setup()
    {
        _contentActions = new ContentActions(InvocationContext, FileManager);
    }

    [TestMethod]
    public async Task SearchContentAsync_ValidRequest_ReturnsContent()
    {
        var request = new SearchContentRequest
        {
            ContentTypeId = "",
            Language = "en"
        };

        var response = await _contentActions!.SearchContentAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.Content.Count > 0);
        
        Console.WriteLine($"Total count: {response.TotalCount}");
        var first3Documents = response.Content.Take(3).ToList();
        Console.WriteLine(JsonConvert.SerializeObject(first3Documents, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadContentAsync_ValidRequest_ReturnsFileReference()
    {
        // Arrange
        var identifier = new ContentLanguageIdentifier
        {
            ContentTypeId = "",
            ContentId = "10",
            Language = "en"
        };

        // Act
        var response = await _contentActions!.DownloadContentAsync(identifier, new(), new()
        {
            ExcludeFields = new List<string> { "data.attributes.contentKey", "data.attributes.status", "data.attributes.translation", "data.attributes.tags1", "data.attributes.tags2" }
        });

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.File);
        Assert.IsFalse(string.IsNullOrEmpty(response.File.Name));
        Assert.AreEqual("text/html", response.File.ContentType);
        
        Console.WriteLine(JsonConvert.SerializeObject(response.File, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadContentAsync_EmptyContentId_ShouldThrowPluginMisconfiguration()
    {
        // Arrange
        var identifier = new ContentLanguageIdentifier
        {
            ContentTypeId = "",
            ContentId = "",
            Language = "en"
        };

        // Act
        var exception = await Assert.ThrowsExceptionAsync<PluginMisconfigurationException>(
            () => _contentActions!.DownloadContentAsync(identifier, new(), new()));

        // Assert
        Assert.IsNotNull(exception);
        Assert.IsNotNull(exception.Message);
    }

    [TestMethod]
    public async Task UploadContentAsync_ValidRequest_UploadsSuccessfully()
    {
        // Arrange
        var request = new UploadContentRequest
        {
            File = new FileReference
            {
                Name = "test.html",
                ContentType = "text/html"
            },
            TargetLanguage = "de"
        };

        // Act & Assert
        await _contentActions!.UploadContentAsync(request);
        Console.WriteLine($"Successfully uploaded content from {request.File.Name} to language {request.TargetLanguage}");
    }
}