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
        var file = await FileManager.DownloadAsync(new() { Name = "Lion [French].html" });
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var html = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Act
        string jsonResult = HtmlToJsonConverter.ConvertToJson(html);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(jsonResult), "JSON result should not be empty");
        
        var jsonObj = JsonConvert.DeserializeObject<JObject>(jsonResult);
        Assert.IsNotNull(jsonObj, "JSON result should be a valid JSON object");
        
        Assert.IsNotNull(jsonObj["data"], "JSON should contain a 'data' property");
        var data = jsonObj["data"] as JObject;
        
        Assert.AreEqual("Lion [French]", data["Name"].ToString(), "Name field doesn't match");
        
        // Check rich text structure
        var description = data["Description"] as JArray;
        Assert.IsNotNull(description, "Description should be an array");
        Assert.IsTrue(description.Count > 0, "Description should have items");
        
        // Verify first heading block
        var headingBlock = description[0] as JObject;
        Assert.AreEqual("heading", headingBlock["type"].ToString(), "First block should be a heading");
        Assert.AreEqual(3, (int)headingBlock["level"], "Heading level should be 3");
        
        var headingChildren = headingBlock["children"] as JArray;
        Assert.IsNotNull(headingChildren, "Heading should have children");
        Assert.AreEqual("Heading 3", headingChildren[0]["text"].ToString(), "Heading text doesn't match");
        
        // Verify paragraph block
        var paragraphBlock = description[1] as JObject;
        Assert.AreEqual("paragraph", paragraphBlock["type"].ToString(), "Second block should be a paragraph");
        
        var paragraphChildren = paragraphBlock["children"] as JArray;
        Assert.IsNotNull(paragraphChildren, "Paragraph should have children");
        Assert.IsTrue(paragraphChildren.Count >= 2, "Paragraph should have at least 2 text nodes");
        Assert.AreEqual("The lion (Panthera leo)", paragraphChildren[0]["text"].ToString(), "First text segment doesn't match");
        
        // Verify Characteristics field (simple string)
        Assert.IsTrue(data["Characteristics"].ToString().Contains("Characteristics (Rich Text)"), 
            "Characteristics field should contain the expected text");
        
        Console.WriteLine("Successfully converted HTML back to JSON:");
        Console.WriteLine(jsonResult);
    }
}
