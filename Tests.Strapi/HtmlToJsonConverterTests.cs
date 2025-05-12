using Apps.Strapi.Utils.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class HtmlToJsonConverterTests : TestBase
{
    [TestMethod]
    public async Task ConvertToJson_ValidHtml_ReturnsExpectedJson()
    {
        // Arrange
        var file = await FileManager.DownloadAsync(new() { Name = "First.html" });
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var html = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Act
        string jsonResult = HtmlToJsonConverter.ConvertToJson(html);
        
        Console.WriteLine("Successfully converted HTML back to JSON:");
        Console.WriteLine(jsonResult);
    }
}
