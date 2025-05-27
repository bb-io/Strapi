using Apps.Strapi.Constants;
using Apps.Strapi.Utils.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}
