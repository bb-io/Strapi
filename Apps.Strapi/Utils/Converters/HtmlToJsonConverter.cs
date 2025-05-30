using Apps.Strapi.Constants;
using Apps.Strapi.Models.Records;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Apps.Strapi.Utils.Converters;

public static class HtmlToJsonConverter
{
    public static HtmlMetadata ExtractMetadata(string metadata)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(metadata);

        var contentIdNode = doc.DocumentNode.SelectSingleNode($"//meta[@name='{MetadataKeys.ContentId}']");
        var contentTypeIdNode = doc.DocumentNode.SelectSingleNode($"//meta[@name='{MetadataKeys.ContentType}']");
        var languageNode = doc.DocumentNode.SelectSingleNode($"//meta[@name='{MetadataKeys.Locale}']");
        if (contentTypeIdNode != null && languageNode != null)
        {
            var contentId = contentIdNode?.GetAttributeValue("content", null);
            var contentTypeId = contentTypeIdNode.GetAttributeValue("content", string.Empty);
            var language = languageNode.GetAttributeValue("content", string.Empty);

            return new HtmlMetadata(contentId, contentTypeId, language);
        }

        throw new PluginApplicationException("HTML document does not contain the required metadata. Make sure the HTML is generated by the connector.");
    }

    public static string ConvertToJson(string html, string strapiVersion, string targetLocale)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            throw new PluginApplicationException("HTML document does not contain a body tag");
        }

        var originalJsonEncoded = bodyNode.GetAttributeValue("original", string.Empty);
        if (string.IsNullOrEmpty(originalJsonEncoded))
        {
            throw new PluginApplicationException("HTML does not contain the original JSON data");
        }

        var originalJson = HttpUtility.HtmlDecode(originalJsonEncoded);
        var jsonObj = JsonConvert.DeserializeObject<JObject>(originalJson);

        if (jsonObj == null)
        {
            throw new PluginApplicationException("Failed to parse original JSON data");
        }

        ProcessPropertyValues(doc, jsonObj);
        ProcessMarkdownContent(doc, jsonObj);
        ProcessRichTextContent(doc, jsonObj);

        var dataObj = jsonObj["data"] as JObject;
        if (dataObj == null)
        {
            throw new PluginApplicationException("Invalid JSON structure: missing 'data' property");
        }

        foreach (var property in JsonProperties.NonLocalizableProperties)
        {
            if (dataObj.ContainsKey(property))
            {
                dataObj.Remove(property);
            }
        }

        if (dataObj.ContainsKey("attributes") && dataObj["attributes"] is JObject attributesObj)
        {
            foreach (var property in attributesObj.Properties())
            {
                if (!JsonProperties.NonLocalizableProperties.Contains(property.Name))
                {
                    dataObj[property.Name] = property.Value;
                }
            }
            
            dataObj.Remove("attributes");
            if (strapiVersion == StrapiVersions.V4)
            {
                dataObj.Add("locale", targetLocale);
            }
        }

        if (strapiVersion == StrapiVersions.V4)
        {
            return JsonConvert.SerializeObject(dataObj);
        }
        
        return JsonConvert.SerializeObject(new { data = dataObj });
    }

    private static void ProcessPropertyValues(HtmlDocument doc, JObject jsonObj)
    {
        var propertyValueNodes = doc.DocumentNode.SelectNodes("//span[@class='property-value'] | //div[@class='property-value']");
        if (propertyValueNodes == null)
            return;

        foreach (var node in propertyValueNodes)
        {
            var jsonPath = node.GetAttributeValue("data-json-path", string.Empty);
            if (string.IsNullOrEmpty(jsonPath))
            {
                continue;
            }

            var isHtmlContent = node.GetAttributeValue("data-html", string.Empty);
            if (isHtmlContent == "true")
            {
                UpdateJsonProperty(jsonObj, jsonPath, node.InnerHtml);
            }
            else
            {
                UpdateJsonProperty(jsonObj, jsonPath, node.InnerText);
            }
        }
    }

    private static void ProcessMarkdownContent(HtmlDocument doc, JObject jsonObj)
    {
        var markdownContainers = doc.DocumentNode.SelectNodes("//div[@class='md-rich-text']");
        if (markdownContainers == null)
        {
            return;
        }

        foreach (var container in markdownContainers)
        {
            var jsonPath = container.GetAttributeValue("data-json-path", string.Empty);
            if (string.IsNullOrEmpty(jsonPath))
            {
                continue;
            }

            var markdownContent = MarkdownConverter.ToMarkdown(container);
            UpdateJsonProperty(jsonObj, jsonPath, markdownContent);
        }
    }

    private static void ProcessRichTextContent(HtmlDocument doc, JObject jsonObj)
    {
        var richTextContainers = doc.DocumentNode.SelectNodes("//div[@class='rich-text-content']");
        if (richTextContainers == null)
        {
            return;
        }

        foreach (var container in richTextContainers)
        {
            var jsonPath = container.GetAttributeValue("data-json-path", string.Empty);
            if (string.IsNullOrEmpty(jsonPath))
            {
                continue;
            }

            var blocks = container.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element).ToList();
            var richTextArray = new JArray();

            foreach (var block in blocks)
            {
                var blockPath = block.GetAttributeValue("data-json-path", string.Empty);
                if (string.IsNullOrEmpty(blockPath))
                {
                    continue;
                }

                var blockJson = RichTextConverter.BuildBlock(block);
                if (blockJson != null)
                {
                    richTextArray.Add(blockJson);
                }
            }

            SetValueAtPath(jsonObj, jsonPath, richTextArray);
        }
    }

    private static void UpdateJsonProperty(JObject json, string path, string value)
    {
        SetValueAtPath(json, path, value);
    }

    private static void SetValueAtPath(JObject json, string path, object value)
    {
        JToken valueToken = value is JToken token ? token : JToken.FromObject(value);

        var pathSegments = ParseJsonPath(path);
        if (pathSegments.Count == 0)
            return;

        JToken current = json;
        for (int i = 0; i < pathSegments.Count - 1; i++)
        {
            var segment = pathSegments[i];

            if (segment.IsArrayIndex)
            {
                if (current![segment.Name] is JArray array && array.Count > segment.Index)
                    current = array[segment.Index];
                else
                    return;
            }
            else
            {
                if (current![segment.Name] == null)
                    return;
                current = current[segment.Name]!;
            }
        }

        var lastSegment = pathSegments[pathSegments.Count - 1];
        if (lastSegment.IsArrayIndex)
        {
            if (current[lastSegment.Name] is JArray array && array.Count > lastSegment.Index)
                array[lastSegment.Index] = valueToken;
        }
        else if (current is JObject obj)
        {
            obj[lastSegment.Name] = valueToken;
        }
    }

    private static List<PathSegment> ParseJsonPath(string path)
    {
        var segments = new List<PathSegment>();
        var parts = path.Split('.');

        foreach (var part in parts)
        {
            if (part.Contains("["))
            {
                var name = part.Substring(0, part.IndexOf('['));
                var indexStr = part.Substring(part.IndexOf('[') + 1, part.IndexOf(']') - part.IndexOf('[') - 1);
                if (int.TryParse(indexStr, out int index))
                {
                    segments.Add(new PathSegment(name, true, index));
                }
            }
            else
            {
                segments.Add(new PathSegment(part, false, 0));
            }
        }

        return segments;
    }

    private class PathSegment
    {
        public string Name { get; }
        public bool IsArrayIndex { get; }
        public int Index { get; }

        public PathSegment(string name, bool isArrayIndex, int index)
        {
            Name = name;
            IsArrayIndex = isArrayIndex;
            Index = index;
        }
    }
}