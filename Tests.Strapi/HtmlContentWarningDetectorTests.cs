using System.Net;
using Apps.Strapi.Constants;
using Apps.Strapi.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests.Strapi;

[TestClass]
public class HtmlContentWarningDetectorTests
{
    [TestMethod]
    public void Analyze_HealthyHtml_ReturnsNoWarnings()
    {
        var originalJson = CreateOriginalJson(
            "<p>Silver remains a diversification tool for long-term investors.</p>",
            "<p>Silver can be traded on WEEX through multiple instruments and strategies. This body stays much longer than the summary and keeps a normal field split for the upload flow.</p>");

        var html = BuildHtml(
            originalJson,
            "<p>La plata sigue siendo una herramienta de diversificacion para inversores a largo plazo.</p>",
            "<p>La plata se puede negociar en WEEX mediante multiples instrumentos y estrategias. Este cuerpo es claramente mas largo que el resumen y mantiene una separacion normal entre campos.</p>");

        var convertedJson = CreateConvertedV4Json(
            "<p>La plata sigue siendo una herramienta de diversificacion para inversores a largo plazo.</p>",
            "<p>La plata se puede negociar en WEEX mediante multiples instrumentos y estrategias. Este cuerpo es claramente mas largo que el resumen y mantiene una separacion normal entre campos.</p>");

        var warnings = HtmlContentWarningDetector.Analyze(html, convertedJson, StrapiVersions.V4);

        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void Analyze_NestedBodyMarker_WarnsAboutFieldBoundaryRisk()
    {
        var originalJson = CreateOriginalJson(
            "<p>Original summary.</p>",
            "<p>Original body with enough content to represent the main article text.</p>");

        var html = BuildHtml(
            originalJson,
            "<p>Translated summary.</p>",
            "<p>Translated body with enough content to represent the main article text.</p>",
            nestBodyInsideSummary: true);

        var convertedJson = CreateConvertedV4Json(
            "<p>Translated summary.</p>",
            "<p>Translated body with enough content to represent the main article text.</p>");

        var warnings = HtmlContentWarningDetector.Analyze(html, convertedJson, StrapiVersions.V4);

        Assert.IsTrue(warnings.Any(x => x.Contains("Field boundary risk detected", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Analyze_SuspiciousHrefMarkup_WarnsAboutBrokenAttribute()
    {
        var originalJson = CreateOriginalJson(
            "<p>Original summary.</p>",
            "<p>Original body with enough content to remain stable.</p>");

        var suspiciousSummary = "<p>If you want to <a href=\"https://www.weex.com/trade/WXT-<a href=\"undefined\">USDT</a>\"><u>buy WXT</u></a> now, you can sign up.</p>";
        var html = BuildHtml(
            originalJson,
            suspiciousSummary,
            "<p>Translated body with enough content to remain stable and separate from the summary.</p>");

        var convertedJson = CreateConvertedV4Json(
            suspiciousSummary,
            "<p>Translated body with enough content to remain stable and separate from the summary.</p>");

        var warnings = HtmlContentWarningDetector.Analyze(html, convertedJson, StrapiVersions.V4);

        Assert.IsTrue(warnings.Any(x => x.Contains("Suspicious HTML attribute detected", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Analyze_ShiftedSummary_WarnsAboutFieldShift()
    {
        var originalJson = CreateOriginalJson(
            "<p>Silver remains a timeless asset for diversification.</p>",
            "<p>Silver stands out as both a precious metal and an industrial commodity. With growing demand from energy and technology sectors, investors often use it for longer narratives and educational content in the body field.</p>");

        var shiftedSummary = """
                             <p>A prata permanece um ativo atemporal para diversificacao.</p>
                             <h2>Por que a prata merece um lugar em seu portfolio</h2>
                             <table><tr><td>Produto</td><td>Tipo</td></tr></table>
                             <ul><li>XAG-USDT</li><li>SLVON-USDT</li></ul>
                             <p>Este conteudo claramente se parece mais com o corpo do artigo do que com um resumo curto.</p>
                             """;

        var shortenedBody = "<p>Corpo restante.</p>";
        var html = BuildHtml(originalJson, shiftedSummary, shortenedBody);
        var convertedJson = CreateConvertedV4Json(shiftedSummary, shortenedBody);

        var warnings = HtmlContentWarningDetector.Analyze(html, convertedJson, StrapiVersions.V4);

        Assert.IsTrue(warnings.Any(x => x.Contains("Potential field shift detected", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Analyze_SummaryDuplicatesBodyPrefix_WarnsAboutSummaryDuplication()
    {
        var originalJson = CreateOriginalJson(
            "<p>Original short summary.</p>",
            "<p>Original body with a much longer explanation that should stay separate from the summary field after upload.</p>");

        var duplicatedSummary = "<p>WEEX lets users trade silver through spot markets and tokenized instruments with competitive fees and strong liquidity for everyday strategies.</p>";
        var body = "<p>WEEX lets users trade silver through spot markets and tokenized instruments with competitive fees and strong liquidity for everyday strategies. The rest of the article continues with more detail, examples, and product descriptions for traders.</p>";
        var html = BuildHtml(originalJson, duplicatedSummary, body);
        var convertedJson = CreateConvertedV4Json(duplicatedSummary, body);

        var warnings = HtmlContentWarningDetector.Analyze(html, convertedJson, StrapiVersions.V4);

        Assert.IsTrue(warnings.Any(x => x.Contains("Potential summary duplication detected", StringComparison.Ordinal)));
    }

    private static string CreateOriginalJson(string summary, string body)
    {
        var payload = new JObject
        {
            ["data"] = new JObject
            {
                ["id"] = 1,
                ["attributes"] = new JObject
                {
                    ["title"] = "Introducing Silver",
                    ["summary"] = summary,
                    ["body"] = body,
                    ["locale"] = "en"
                }
            },
            ["meta"] = new JObject()
        };

        return JsonConvert.SerializeObject(payload);
    }

    private static string CreateConvertedV4Json(string summary, string body)
    {
        var payload = new JObject
        {
            ["title"] = "Introducing Silver",
            ["summary"] = summary,
            ["body"] = body,
            ["locale"] = "pt-PT"
        };

        return JsonConvert.SerializeObject(payload);
    }

    private static string BuildHtml(string originalJson, string summaryHtml, string bodyHtml, bool nestBodyInsideSummary = false)
    {
        var encodedOriginalJson = WebUtility.HtmlEncode(originalJson);

        var fieldMarkup = nestBodyInsideSummary
            ? $"""
               <div class="property-value" data-json-path="data.attributes.summary" data-html="true">
               {summaryHtml}
               <div class="property-value" data-json-path="data.attributes.body" data-html="true">{bodyHtml}</div>
               </div>
               """
            : $"""
               <div class="property-value" data-json-path="data.attributes.summary" data-html="true">{summaryHtml}</div>
               <div class="property-value" data-json-path="data.attributes.body" data-html="true">{bodyHtml}</div>
               """;

        return $$"""
                 <!DOCTYPE html>
                 <html>
                 <head>
                   <meta charset="UTF-8">
                   <meta name="blackbird-content-type" content="articles">
                   <meta name="blackbird-locale" content="en">
                 </head>
                 <body original="{{encodedOriginalJson}}">
                   <div class="json-object" data-json-path="data.attributes">
                     {{fieldMarkup}}
                   </div>
                 </body>
                 </html>
                 """;
    }
}
