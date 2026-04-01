using System.Net;
using System.Text.RegularExpressions;
using Apps.Strapi.Constants;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Utils;

public static partial class HtmlContentWarningDetector
{
    private static readonly string[] SummaryStructuralTags = ["h1", "h2", "h3", "h4", "h5", "h6", "table", "figure", "ul", "ol", "li"];
    private const double SummaryBodyRatioWarningThreshold = 0.2;
    private const double SevereSummaryBodyRatioWarningThreshold = 0.5;
    private const int MinimumMeaningfulTextLength = 80;

    public static List<string> Analyze(string html, string convertedJson, string strapiVersion)
    {
        var warnings = new List<string>();

        try
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return warnings;
            }

            html = NormalizeHtmlTags(html);
            DetectSuspiciousRawHtml(html, warnings);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            DetectHtmlParseIssues(document, warnings);

            var originalJson = TryExtractOriginalJson(document);
            if (originalJson == null)
            {
                return warnings;
            }

            DetectMarkerWarnings(document, originalJson, warnings);
            DetectContentWarnings(originalJson, convertedJson, strapiVersion, warnings);
        }
        catch
        {
            // Warning detection must never block the upload flow.
        }

        return warnings;
    }

    private static void DetectSuspiciousRawHtml(string html, List<string> warnings)
    {
        foreach (Match match in SuspiciousAttributeRegex().Matches(html).Cast<Match>().Take(3))
        {
            AddWarning(
                warnings,
                $"Suspicious HTML attribute detected: an href/src attribute contains embedded markup, which may indicate a broken tag. Snippet: {ToSnippet(match.Value)}");
        }
    }

    private static void DetectHtmlParseIssues(HtmlDocument document, List<string> warnings)
    {
        foreach (var parseError in document.ParseErrors.Take(3))
        {
            AddWarning(
                warnings,
                $"HTML parse issue detected at line {parseError.Line}, position {parseError.LinePosition}: {parseError.Reason}.");
        }
    }

    private static JObject? TryExtractOriginalJson(HtmlDocument document)
    {
        var bodyNode = document.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            return null;
        }

        var originalJsonEncoded = bodyNode.GetAttributeValue("original", string.Empty);
        if (string.IsNullOrWhiteSpace(originalJsonEncoded))
        {
            return null;
        }

        var originalJson = WebUtility.HtmlDecode(originalJsonEncoded);
        return JObject.Parse(originalJson);
    }

    private static void DetectMarkerWarnings(HtmlDocument document, JObject originalJson, List<string> warnings)
    {
        var brokenMarkerNodes = document.DocumentNode.SelectNodes("//*[@data-blackbird-key and not(@data-json-path)]");
        if (brokenMarkerNodes != null && brokenMarkerNodes.Count > 0)
        {
            var examples = brokenMarkerNodes
                .Select(x => x.GetAttributeValue("data-blackbird-key", string.Empty))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3);

            AddWarning(
                warnings,
                $"Malformed field markers detected: {brokenMarkerNodes.Count} node(s) have data-blackbird-key but no data-json-path. Examples: {string.Join(", ", examples)}.");
        }

        var originalContent = GetOriginalContentObject(originalJson);
        if (originalContent == null)
        {
            return;
        }

        var basePath = GetBaseFieldPath(originalJson);
        var summaryPath = $"{basePath}.summary";
        var bodyPath = $"{basePath}.body";

        var summaryNodes = SelectNodesByJsonPath(document, summaryPath);
        var bodyNodes = SelectNodesByJsonPath(document, bodyPath);

        if (originalContent["summary"]?.Type == JTokenType.String)
        {
            DetectFieldMarkerCardinalityWarnings("summary", summaryNodes, warnings);
        }

        if (originalContent["body"]?.Type == JTokenType.String)
        {
            DetectFieldMarkerCardinalityWarnings("body", bodyNodes, warnings);
        }

        var summaryNode = summaryNodes?.FirstOrDefault();
        var bodyNode = bodyNodes?.FirstOrDefault();

        if (summaryNode != null && bodyNode != null)
        {
            if (IsAncestor(summaryNode, bodyNode))
            {
                AddWarning(
                    warnings,
                    "Field boundary risk detected: 'body' is nested inside 'summary' in the uploaded HTML. This can cause content to leak between fields.");
            }
            else if (IsAncestor(bodyNode, summaryNode))
            {
                AddWarning(
                    warnings,
                    "Field boundary risk detected: 'summary' is nested inside 'body' in the uploaded HTML. This can cause content to leak between fields.");
            }
        }
    }

    private static void DetectFieldMarkerCardinalityWarnings(string fieldName, List<HtmlNode>? nodes, List<string> warnings)
    {
        if (nodes == null || nodes.Count == 0)
        {
            AddWarning(warnings, $"Expected field marker for '{fieldName}' was not found in the uploaded HTML.");
            return;
        }

        if (nodes.Count > 1)
        {
            AddWarning(warnings, $"Expected a single field marker for '{fieldName}', but found {nodes.Count}.");
        }
    }

    private static void DetectContentWarnings(JObject originalJson, string convertedJson, string strapiVersion, List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(convertedJson))
        {
            return;
        }

        var originalContent = GetOriginalContentObject(originalJson);
        var convertedContent = GetConvertedContentObject(convertedJson, strapiVersion);
        if (originalContent == null || convertedContent == null)
        {
            return;
        }

        var originalSummaryHtml = originalContent["summary"]?.ToString();
        var originalBodyHtml = originalContent["body"]?.ToString();
        var currentSummaryHtml = convertedContent["summary"]?.ToString();
        var currentBodyHtml = convertedContent["body"]?.ToString();

        if (string.IsNullOrWhiteSpace(originalSummaryHtml) ||
            string.IsNullOrWhiteSpace(originalBodyHtml) ||
            string.IsNullOrWhiteSpace(currentSummaryHtml) ||
            string.IsNullOrWhiteSpace(currentBodyHtml))
        {
            return;
        }

        DetectFieldShiftWarnings(originalSummaryHtml, originalBodyHtml, currentSummaryHtml, currentBodyHtml, warnings);
        DetectSummaryDuplicationWarnings(currentSummaryHtml, currentBodyHtml, warnings);
    }

    private static void DetectFieldShiftWarnings(
        string originalSummaryHtml,
        string originalBodyHtml,
        string currentSummaryHtml,
        string currentBodyHtml,
        List<string> warnings)
    {
        var originalSummaryText = ExtractText(originalSummaryHtml);
        var originalBodyText = ExtractText(originalBodyHtml);
        var currentSummaryText = ExtractText(currentSummaryHtml);
        var currentBodyText = ExtractText(currentBodyHtml);

        if (originalBodyText.Length < MinimumMeaningfulTextLength || currentSummaryText.Length < MinimumMeaningfulTextLength)
        {
            return;
        }

        var originalRatio = originalSummaryText.Length / (double)Math.Max(originalBodyText.Length, 1);
        var currentRatio = currentSummaryText.Length / (double)Math.Max(currentBodyText.Length, 1);
        var structuralTags = GetStructuralTags(currentSummaryHtml);
        var originalStructuralTags = GetStructuralTags(originalSummaryHtml);
        var newStructuralTags = structuralTags
            .Except(originalStructuralTags, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var ratioSuspicious =
            currentRatio >= SummaryBodyRatioWarningThreshold &&
            currentRatio >= originalRatio + 0.15 &&
            (currentRatio >= originalRatio * 2.5 || newStructuralTags.Count > 0);

        if (currentRatio >= SevereSummaryBodyRatioWarningThreshold || ratioSuspicious)
        {
            var tagSuffix = newStructuralTags.Count > 0
                ? $" Summary now contains block tags: [{string.Join(", ", newStructuralTags)}]."
                : string.Empty;

            AddWarning(
                warnings,
                $"Potential field shift detected: summary/body text ratio is {currentRatio:0.00} after upload (original {originalRatio:0.00}).{tagSuffix}");
        }
        else if (newStructuralTags.Count > 0 && currentSummaryText.Length >= MinimumMeaningfulTextLength)
        {
            AddWarning(
                warnings,
                $"Summary contains unexpected block tags after upload: [{string.Join(", ", newStructuralTags)}]. This may indicate a broken field boundary.");
        }
    }

    private static void DetectSummaryDuplicationWarnings(string currentSummaryHtml, string currentBodyHtml, List<string> warnings)
    {
        var summaryText = NormalizeForComparison(ExtractText(currentSummaryHtml));
        var bodyText = NormalizeForComparison(ExtractText(currentBodyHtml));

        if (summaryText.Length < MinimumMeaningfulTextLength || bodyText.Length < summaryText.Length)
        {
            return;
        }

        var commonPrefixLength = GetCommonPrefixLength(summaryText, bodyText);
        var prefixRatio = commonPrefixLength / (double)Math.Max(summaryText.Length, 1);
        var summaryBodyRatio = summaryText.Length / (double)Math.Max(bodyText.Length, 1);

        if (prefixRatio >= 0.85 && summaryBodyRatio >= 0.2)
        {
            AddWarning(
                warnings,
                $"Potential summary duplication detected: summary matches the opening {prefixRatio:P0} of the body text.");
        }
    }

    private static JObject? GetOriginalContentObject(JObject originalJson)
    {
        var dataObject = originalJson["data"] as JObject;
        if (dataObject == null)
        {
            return null;
        }

        return dataObject["attributes"] as JObject ?? dataObject;
    }

    private static JObject? GetConvertedContentObject(string convertedJson, string strapiVersion)
    {
        var convertedObject = JObject.Parse(convertedJson);
        if (strapiVersion == StrapiVersions.V4)
        {
            return convertedObject;
        }

        return convertedObject["data"] as JObject;
    }

    private static string GetBaseFieldPath(JObject originalJson)
    {
        var dataObject = originalJson["data"] as JObject;
        var hasAttributes = dataObject?["attributes"] != null;
        return hasAttributes ? "data.attributes" : "data";
    }

    private static List<HtmlNode>? SelectNodesByJsonPath(HtmlDocument document, string jsonPath)
    {
        var nodes = document.DocumentNode.SelectNodes($"//*[@data-json-path='{jsonPath}']");
        return nodes?.ToList();
    }

    private static bool IsAncestor(HtmlNode parent, HtmlNode child)
    {
        for (var current = child.ParentNode; current != null; current = current.ParentNode)
        {
            if (current == parent)
            {
                return true;
            }
        }

        return false;
    }

    private static List<string> GetStructuralTags(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document.DocumentNode
            .Descendants()
            .Where(x => SummaryStructuralTags.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
            .Select(x => x.Name.ToLowerInvariant())
            .Distinct()
            .ToList();
    }

    private static string ExtractText(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var text = HtmlEntity.DeEntitize(document.DocumentNode.InnerText ?? string.Empty) ?? string.Empty;
        return Regex.Replace(text, "\\s+", " ").Trim();
    }

    private static string NormalizeForComparison(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Regex.Replace(value, "\\s+", " ").Trim().ToLowerInvariant();
    }

    private static int GetCommonPrefixLength(string first, string second)
    {
        var maxLength = Math.Min(first.Length, second.Length);
        var index = 0;

        while (index < maxLength && first[index] == second[index])
        {
            index++;
        }

        return index;
    }

    private static string NormalizeHtmlTags(string html)
    {
        return html
            .Replace("пјњ", "<")
            .Replace("пјћ", ">")
            .Replace("пј‚", "\"")
            .Replace("'", "'")
            .Replace("'", "'")
            .Replace("пјЏ", "/")
            .Replace("пјќ", "=");
    }

    private static string ToSnippet(string value)
    {
        var normalized = Regex.Replace(value, "\\s+", " ").Trim();
        return normalized.Length <= 140 ? normalized : normalized[..140] + "...";
    }

    private static void AddWarning(List<string> warnings, string warning)
    {
        if (!warnings.Contains(warning, StringComparer.OrdinalIgnoreCase))
        {
            warnings.Add(warning);
        }
    }

    [GeneratedRegex(@"(?:href|src)\s*=\s*[""'][^""']*[<>][^""']*[""']", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SuspiciousAttributeRegex();
}
