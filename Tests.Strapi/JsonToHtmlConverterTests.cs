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
                },
                {
                    ""type"": ""link"",
                    ""url"": ""https://ngrok.com/docs/getting-started/"",
                    ""children"": [
                        {
                            ""type"": ""text"",
                            ""text"": ""Ngrok Docs""
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
        var html = JsonToHtmlConverter.ConvertToHtml(_testJson, contentId, "animals", null);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(html), "HTML should not be empty");
        
        var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html));
        memoryStream.Position = 0;
        await FileManager.UploadAsync(memoryStream, "text/html", "strapi_converter_test.html");
        
        Console.WriteLine("HTML generated and saved to strapi_converter_test.html");
    }

    [TestMethod]
    public void ConvertToHtml_InvalidJson_ThrowsException()
    {
        string invalidJson = @"{""invalid"": ""no data object""}";
        string contentId = "test-content";

        Assert.ThrowsException<ArgumentException>(() => 
            JsonToHtmlConverter.ConvertToHtml(invalidJson, contentId, "animals", null),
            "Should throw exception for invalid JSON structure");
    }
}
