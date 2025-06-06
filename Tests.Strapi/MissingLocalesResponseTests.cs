using Apps.Strapi.Models.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Tests.Strapi.Base;

namespace Tests.Strapi;

[TestClass]
public class MissingLocalesResponseTests : TestBase
{
    [TestMethod]
    public void GetLocalesFromJObject_ValidJson_ReturnsLocales()
    {
        // Arrange
        string jsonString = @"
        {
            ""data"": {
                ""article"": {
                    ""data"": {
                        ""attributes"": {
                            ""localizations"": {
                                ""data"": [
                                    {
                                        ""id"": ""316"",
                                        ""attributes"": {
                                            ""locale"": ""zh-Hans""
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        }";
        
        JObject jObject = JObject.Parse(jsonString);
        string contentType = "article";

        // Act
        var locales = MissingLocalesResponse.GetLocalesFromJObject(jObject, contentType);

        // Assert
        Assert.IsNotNull(locales);
        Assert.AreEqual(1, locales.Count);
        Assert.AreEqual("zh-Hans", locales[0]);
        
        Console.WriteLine($"Successfully extracted {locales.Count} locale(s): {string.Join(", ", locales)}");
    }
    
    [TestMethod]
    public void GetLocalesFromJObject_DifferentContentType_ReturnsEmptyList()
    {
        // Arrange
        string jsonString = @"
        {
            ""data"": {
                ""article"": {
                    ""data"": {
                        ""attributes"": {
                            ""localizations"": {
                                ""data"": [
                                    {
                                        ""id"": ""316"",
                                        ""attributes"": {
                                            ""locale"": ""zh-Hans""
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        }";
        
        JObject jObject = JObject.Parse(jsonString);
        string contentType = "blog"; // Different content type than what's in the JSON

        // Act
        var locales = MissingLocalesResponse.GetLocalesFromJObject(jObject, contentType);

        // Assert
        Assert.IsNotNull(locales);
        Assert.AreEqual(0, locales.Count);
    }
}
