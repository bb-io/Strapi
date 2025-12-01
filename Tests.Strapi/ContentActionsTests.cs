using Apps.Strapi.Actions;
using Apps.Strapi.Constants;
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
            ContentTypeIds = ["articles"],
            Language = "en",
            Status = "draft",
            StrapiVersion = StrapiVersions.V4,
            FieldNames = ["data.attributes.contentKey"],
            FieldValues = ["article_306"]
        };

        var response = await _contentActions!.SearchContentAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.Content.Count > 0);
        
        Console.WriteLine($"Total count: {response.TotalCount}");
        var first3Documents = response.Content.Take(3);
        Console.WriteLine(JsonConvert.SerializeObject(first3Documents, Formatting.Indented));
    }

    [TestMethod]
    public async Task SearchContentAsync_RequestWithFieldFilter_ReturnsContent()
    {
        var request = new SearchContentRequest
        {
            ContentTypeIds = ["animals"],
            FieldNames = ["data.attributes.translation"],
            FieldValues = ["Human Translation"]
        };

        var response = await _contentActions!.SearchContentAsync(request);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.Content.Count > 0);
        Console.WriteLine($"Total count: {response.TotalCount}");
    }

    [TestMethod]
    public async Task DownloadContentAsync_ValidRequest_ReturnsFileReference()
    {
        // Arrange
        var identifier = new ContentLanguageIdentifier
        {
            ContentTypeId = "learns",
            ContentId = "9047",
            //Language = "en"
        };

        // Act
        var response = await _contentActions!.DownloadContentAsync(identifier, new(), new()
        {
            ExcludeFields = ["data.attributes.status"]
        });

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Content);
        Assert.IsFalse(string.IsNullOrEmpty(response.Content.Name));
        Assert.AreEqual("text/html", response.Content.ContentType);
        
        Console.WriteLine(JsonConvert.SerializeObject(response.Content, Formatting.Indented));
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
            Content = new FileReference
            {
                Name = "1229047.html",
                ContentType = "text/html"
            },
            Locale = "fr-FR1",
            StrapiVersion = "v4"
        };

        // Act & Assert
        var result = await _contentActions!.UploadContentAsync(request);
        Console.WriteLine($"Successfully uploaded content from {request.Content.Name} to language {request.Locale}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetTextFieldValueAsync_ValidRequest_ReturnsFieldValue()
    {
        // Arrange
        var identifier = new ContentIdentifier
        {
            ContentTypeId = "animals",
            ContentId = "6"
        };
        var request = new GetTextFieldValueRequest
        {
            FieldPath = "data.attributes.translation"
        };

        // Act
        var response = await _contentActions!.GetTextFieldValueAsync(identifier, request);

        // Assert
        Assert.IsNotNull(response.Value);
        Console.WriteLine($"Field value: {response.Value}");
    }

    [TestMethod]
    public async Task GetMissingLocalesAsync_ValidRequest_ReturnsMissingLocales()
    {
        // Arrange
        var identifier = new GetMissingLocalesRequest
        {
            ContentTypeId = "animals",
            ContentId = "14116",
            StrapiVersion = "v4"
        };

        // Act
        var response = await _contentActions!.GetMissingLocalesAsync(identifier);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.MissingLocales);
        Assert.IsTrue(response.MissingLocales.Count > 0);
        
        // Output for debugging
        Console.WriteLine($"Missing locales count: {response.MissingLocales.Count}");
        Console.WriteLine($"Missing locales: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
    }

    [TestMethod]
    public async Task UpdateContentAsync_ValidRequest_UpdatesContentSuccessfully()
    {
        // Arrange
        var identifier = new ContentIdentifier
        {
            ContentTypeId = "articles",
            ContentId = "kdvapq3g7rsig6s3jk6a68xb"
        };
        var request = new UpdateContentRequest
        {
            FieldNames = ["LocalizationState"],
            FieldValues = ["In progress"]
        };

        // Act
        var response = await _contentActions!.UpdateContentAsync(identifier, request);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Id);
        Assert.AreEqual(identifier.ContentTypeId, response.ContentTypeId);

        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }
}