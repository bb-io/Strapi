using Apps.Strapi.Constants;
using Apps.Strapi.Models.Records;
using Apps.Strapi.Utils.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Text;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class HtmlToJsonConverterTests : TestBase
{
    [TestMethod]
    public async Task ConvertToJson_ValidHtml_ReturnsExpectedJson()
    {
        // Arrange
        var file = await FileManager.DownloadAsync(new() { Name = "Blackbird 1.html" });
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var html = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Act
        string jsonResult = HtmlToJsonConverter.ConvertToJson(html, StrapiVersions.V5, "de");
        
        Console.WriteLine("Successfully converted HTML back to JSON:");
        Console.WriteLine(jsonResult);
    }

    [TestMethod]
    public void RoundTrip_V5_NormalizesReferenceFields_AndRemovesLocalizations()
    {
        var sourceJson = """
                         {
                           "data": {
                             "id": 42,
                             "documentId": "entry-42",
                             "title": "Original title",
                             "body": "<p>Original body</p>",
                             "section": {
                               "data": {
                                 "id": 7,
                                 "documentId": "section-7",
                                 "name": "News"
                               }
                             },
                             "seo": {
                               "metaTitle": "SEO title",
                               "metaDescription": "SEO description"
                             },
                             "localizations": {
                               "data": [
                                 {
                                   "id": 43,
                                   "documentId": "entry-43",
                                   "locale": "fr"
                                 }
                               ]
                             },
                             "locale": "en"
                           },
                           "meta": {}
                         }
                         """;

        var html = JsonToHtmlConverter.ConvertToHtml(
            sourceJson,
            new HtmlGenerationMetadata("42", "articles", "en", "entry-42", "Original title", null, "https://example.com"),
            null,
            null);

        Assert.IsFalse(html.Contains("localizations", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(html.Contains("entry-43", StringComparison.OrdinalIgnoreCase));

        html = html
            .Replace("Original title", "Translated title")
            .Replace("Original body", "Translated body")
            .Replace("SEO title", "SEO translated title")
            .Replace("SEO description", "SEO translated description");

        var jsonResult = HtmlToJsonConverter.ConvertToJson(html, StrapiVersions.V5, "de");
        var payload = JObject.Parse(jsonResult);
        var data = (JObject)payload["data"]!;

        Assert.AreEqual("Translated title", data["title"]?.ToString());
        Assert.AreEqual("<p>Translated body</p>", data["body"]?.ToString());
        Assert.AreEqual("SEO translated title", data["seo"]?["metaTitle"]?.ToString());
        Assert.AreEqual("SEO translated description", data["seo"]?["metaDescription"]?.ToString());
        Assert.IsNull(data["localizations"]);
        Assert.AreEqual("7", data["section"]?.ToString());
        Assert.AreEqual(JTokenType.Integer, data["section"]?.Type);
    }
}
