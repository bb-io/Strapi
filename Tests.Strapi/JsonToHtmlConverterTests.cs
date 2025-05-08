using Apps.Strapi.Utils.Converters;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class JsonToHtmlConverterTests : TestBase
{
    private readonly string _testJson = @"{
        ""data"": {
            ""id"": 1,
            ""documentId"": ""owo7ldexvq0ib6iaa4osf3ez"",
            ""Name"": ""Lion [English]"",
            ""Description"": [
                {
                    ""type"": ""heading"",
                    ""children"": [
                        {
                            ""type"": ""text"",
                            ""text"": ""Heading 3"",
                            ""bold"": true
                        }
                    ],
                    ""level"": 3
                },
                {
                    ""type"": ""paragraph"",
                    ""children"": [
                        {
                            ""type"": ""text"",
                            ""bold"": true,
                            ""text"": ""The lion (Panthera leo)""
                        },
                        {
                            ""type"": ""text"",
                            ""text"": "" is a large carnivorous feline native to parts of Africa and Asia. Known as the \""king of the jungle,\"" it is distinguished by its muscular build, deep chest, and a mane that encircles the face of males. Lions typically live in savannas, grasslands, and open woodlands. They are social animals, forming groups called prides, which usually consist of related females and their offspring along with a small number of adult males.""
                        }
                    ]
                }
            ],
            ""Characteristics"": ""### 🦁 Characteristics (Rich Text)\n\n- Size: Males can weigh up to 250 kg, while females typically weigh around 150 kg.\n- Mane: Males possess a prominent mane, which varies in color and length.\n- Roar: Lions have a powerful roar that can be heard up to 8 kilometers away.\n- Diet: Carnivorous; primarily hunt large herbivores such as zebras and wildebeests.\n- Behavior: Lions are mostly nocturnal and spend a significant part of the day resting.\n- Lifespan: In the wild, they live around 10–14 years; longer in captivity."",
            ""createdAt"": ""2025-05-07T08:38:52.665Z"",
            ""updatedAt"": ""2025-05-08T08:10:11.410Z"",
            ""publishedAt"": null,
            ""locale"": ""en""
        },
        ""meta"": {}
    }";

    [TestMethod]
    public async Task ConvertToHtml_ValidJson_ReturnsExpectedHtml()
    {
        // Arrange
        string contentId = "animals:lion";

        // Act
        var html = JsonToHtmlConverter.ConvertToHtml(_testJson, contentId);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(html), "HTML should not be empty");
        
        // Parse the HTML to validate structure
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        
        // Check HTML structure
        Assert.IsNotNull(htmlDoc.DocumentNode.SelectSingleNode("//html"), "Missing <html> tag");
        Assert.IsNotNull(htmlDoc.DocumentNode.SelectSingleNode("//head"), "Missing <head> tag");
        Assert.IsNotNull(htmlDoc.DocumentNode.SelectSingleNode("//body"), "Missing <body> tag");
        
        // Check metadata
        var contentIdMeta = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-content-id']");
        Assert.IsNotNull(contentIdMeta, "Missing content-id metadata");
        Assert.AreEqual(contentId, contentIdMeta.GetAttributeValue("content", ""), "Content ID doesn't match");
        
        var contentTypeMeta = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-content-type']");
        Assert.IsNotNull(contentTypeMeta, "Missing content-type metadata");
        Assert.AreEqual("entry", contentTypeMeta.GetAttributeValue("content", ""), "Content type should be 'entry'");
        
        var localeMeta = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-locale']");
        Assert.IsNotNull(localeMeta, "Missing locale metadata");
        Assert.AreEqual("en", localeMeta.GetAttributeValue("content", ""), "Locale should be 'en'");
        
        // Check JSON storage
        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
        Assert.IsTrue(bodyNode.Attributes.Contains("original"), "Body should contain original JSON");
        
        // Check for localizable content
        Assert.IsTrue(html.Contains("Lion [English]"), "Name field is missing");
        Assert.IsTrue(html.Contains("The lion (Panthera leo)"), "Description content missing");
        Assert.IsTrue(html.Contains("Characteristics (Rich Text)"), "Characteristics content missing");
        
        // Check for data-json-path attributes
        var jsonPathElements = htmlDoc.DocumentNode.SelectNodes("//*[@data-json-path]");
        Assert.IsTrue(jsonPathElements.Count > 0, "No elements with data-json-path found");
        
        // Save HTML for examination (optional)
        var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html));
        memoryStream.Position = 0;
        await FileManager.UploadAsync(memoryStream, "text/html", "strapi_converter_test.html");
        
        Console.WriteLine("HTML generated and saved to strapi_converter_test.html");
    }

    [TestMethod]
    public void ConvertToHtml_InvalidJson_ThrowsException()
    {
        // Arrange
        string invalidJson = @"{""invalid"": ""no data object""}";
        string contentId = "test-content";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => 
            JsonToHtmlConverter.ConvertToHtml(invalidJson, contentId),
            "Should throw exception for invalid JSON structure");
    }
}
