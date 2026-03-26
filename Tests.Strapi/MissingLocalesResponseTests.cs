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
                ""id"": ""14402"",
                ""attributes"": {
                    ""locale"": ""en"",
                    ""localizations"": {
                        ""data"": [
                            {
                                ""id"": ""316"",
                                ""attributes"": {
                                    ""locale"": ""zh-Hans""
                                }
                            },
                            {
                                ""id"": ""317"",
                                ""attributes"": {
                                    ""locale"": ""ja""
                                }
                            }
                        ]
                    }
                }
            }
        }";
        
        JObject jObject = JObject.Parse(jsonString);

        // Act
        var locales = MissingLocalesResponse.GetLocalesFromJObject(jObject);

        // Assert
        Assert.IsNotNull(locales);
        CollectionAssert.AreEquivalent(new[] { "en", "zh-Hans", "ja" }, locales);
        
        Console.WriteLine($"Successfully extracted {locales.Count} locale(s): {string.Join(", ", locales)}");
    }

    [TestMethod]
    public void GetIdsWithLocalesFromJObject_ValidRestJson_ReturnsIdsWithLocales()
    {
        // Arrange
        string jsonString = @"
        {
            ""data"": {
                ""id"": ""14402"",
                ""attributes"": {
                    ""locale"": ""en"",
                    ""localizations"": {
                        ""data"": [
                            {
                                ""id"": ""316"",
                                ""attributes"": {
                                    ""locale"": ""zh-Hans""
                                }
                            },
                            {
                                ""id"": ""317"",
                                ""attributes"": {
                                    ""locale"": ""ja""
                                }
                            }
                        ]
                    }
                }
            }
        }";
        
        JObject jObject = JObject.Parse(jsonString);

        // Act
        var idsWithLocales = MissingLocalesResponse.GetIdsWithLocalesFromJObject(jObject);

        // Assert
        Assert.IsNotNull(idsWithLocales);
        Assert.AreEqual(3, idsWithLocales.Count);
        CollectionAssert.AreEquivalent(new[] { "en", "zh-Hans", "ja" }, idsWithLocales.Select(x => x.Locale).ToList());
    }

    [TestMethod]
    public void GetLocalesFromJObject_MissingData_ReturnsEmptyList()
    {
        // Arrange
        string jsonString = @"
        {
            ""meta"": {}
        }";
        
        JObject jObject = JObject.Parse(jsonString);

        // Act
        var locales = MissingLocalesResponse.GetLocalesFromJObject(jObject);

        // Assert
        Assert.IsNotNull(locales);
        Assert.AreEqual(0, locales.Count);
    }
    
    [TestMethod]
    public void GetLocalesFromJObject_MissingLocalizations_ReturnsCurrentLocaleOnly()
    {
        // Arrange
        string jsonString = @"
        {
            ""data"": {
                ""id"": ""14402"",
                ""attributes"": {
                    ""locale"": ""en""
                }
            }
        }";
        
        JObject jObject = JObject.Parse(jsonString);

        // Act
        var locales = MissingLocalesResponse.GetLocalesFromJObject(jObject);

        // Assert
        Assert.IsNotNull(locales);
        CollectionAssert.AreEquivalent(new[] { "en" }, locales);
    }
}
