using Apps.Strapi.Actions;
using Apps.Strapi.Models.Identifiers;
using Apps.Strapi.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class ContentActionsTests : TestBase
{
    private ContentActions _contentActions;

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
            ContentTypeId = "animals",
            Language = "en",
            Status = "published"
        };

        var response = await _contentActions.SearchContentAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.Content.Count > 0);
        
        Console.WriteLine($"Total count: {response.TotalCount}");
        foreach (var document in response.Content)
        {
            Console.WriteLine($"Document ID: {document.DocumentId}, Created at: {document.CreatedAt}");
        }
    }

    [TestMethod]
    public async Task DownloadContentAsync_ValidRequest_ReturnsFileReference()
    {
        // Arrange
        var identifier = new ContentLanguageIdentifier
        {
            ContentTypeId = "majors",
            ContentId = "ll0zpo4ouq4hh4adhd84l8k0",
            Language = "en"
        };

        // Act
        var response = await _contentActions.DownloadContentAsync(identifier);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.File);
        Assert.IsFalse(string.IsNullOrEmpty(response.File.Name));
        Assert.AreEqual("text/html", response.File.ContentType);
        
        Console.WriteLine(JsonConvert.SerializeObject(response.File, Formatting.Indented));
    }

    [TestMethod]
    public async Task UploadContentAsync_ValidRequest_UploadsSuccessfully()
    {
        // Arrange
        var request = new UploadContentRequest
        {
            File = new FileReference
            {
                Name = "First.html",
                ContentType = "text/html"
            },
            TargetLanguage = "fr"
        };

        // Act & Assert
        await _contentActions.UploadContentAsync(request);
        
        Console.WriteLine($"Successfully uploaded content from {request.File.Name} to language {request.TargetLanguage}");
    }
}
