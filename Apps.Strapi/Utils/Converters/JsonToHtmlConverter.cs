using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using Apps.Strapi.Constants;

namespace Apps.Strapi.Utils.Converters;

public static class JsonToHtmlConverter
{
    public static string ExtractTitle(string json, string defaultName)
    {
        var jsonObj = JsonConvert.DeserializeObject<JObject>(json)!;
        var dataObj = jsonObj["data"] as JObject;

        if (dataObj == null)
        {
            throw new ArgumentException("Invalid JSON structure. Expected 'data' property.");
        }

        var title = dataObj["Name"]?.ToString() ?? dataObj["Title"]?.ToString() ?? defaultName;
        return title;
    }

    public static string ConvertToHtml(string json, string? contentId, string contentType, IEnumerable<string>? nonLocalizableFields)
    {
        var jsonObj = JsonConvert.DeserializeObject<JObject>(json)!;
        var dataObj = jsonObj["data"] as JObject;

        if (dataObj == null)
        {
            ExceptionExtensions.ThrowIfNullOrEmpty(contentId, "Content ID");
            throw new ArgumentException("Invalid JSON structure. Expected 'data' property.");
        }

        var doc = new HtmlDocument();
        CreateHtmlStructure(doc, jsonObj, dataObj, contentId, contentType);

        ProcessJsonObject(dataObj, doc.DocumentNode.SelectSingleNode("//body"), doc, "data", nonLocalizableFields);
        return "<!DOCTYPE html>\n" + doc.DocumentNode.OuterHtml;
    }

    private static void CreateHtmlStructure(HtmlDocument doc, JObject jsonObj, JObject dataObj, string? contentId, string contentType)
    {
        var htmlNode = CreateElement(doc, "html");
        doc.DocumentNode.AppendChild(htmlNode);

        var headNode = CreateElement(doc, "head");
        htmlNode.AppendChild(headNode);

        AddMetaTag(doc, headNode, "charset", "UTF-8");
        if (!string.IsNullOrEmpty(contentId))
        {
            AddMetaTag(doc, headNode, MetadataKeys.ContentId, contentId);
        }

        AddMetaTag(doc, headNode, MetadataKeys.ContentType, contentType);

        string locale = dataObj["locale"]?.ToString() ?? "en";
        AddMetaTag(doc, headNode, MetadataKeys.Locale, locale);

        var bodyNode = CreateElement(doc, "body");
        bodyNode.SetAttributeValue("original", HttpUtility.HtmlEncode(jsonObj.ToString(Formatting.None)));
        htmlNode.AppendChild(bodyNode);
    }

    private static void AddMetaTag(HtmlDocument doc, HtmlNode parentNode, string name, string content)
    {
        var metaTag = CreateElement(doc, "meta");

        if (name == "charset")
            metaTag.SetAttributeValue(name, content);
        else
        {
            metaTag.SetAttributeValue("name", name);
            metaTag.SetAttributeValue("content", content);
        }

        parentNode.AppendChild(metaTag);
    }

    private static void ProcessJsonObject(JObject jsonObj, HtmlNode parentNode, HtmlDocument doc, string jsonPath, IEnumerable<string>? nonLocalizableFields)
    {
        foreach (var property in jsonObj.Properties())
        {
            string currentPath = AppendJsonPath(jsonPath, property.Name);

            if (JsonProperties.NonLocalizableProperties.Contains(property.Name) || nonLocalizableFields?.Contains(property.Path) == true)
            {
                continue;
            }

            if (property.Value?.Type == JTokenType.Object)
            {
                var container = CreateContainerElement(doc, "div", "json-object", currentPath);
                parentNode.AppendChild(container);
                ProcessJsonObject((JObject)property.Value, container, doc, currentPath, nonLocalizableFields);
            }
            else if (property.Value?.Type == JTokenType.Array)
            {
                var container = CreateContainerElement(doc, "div", "json-array", currentPath);
                parentNode.AppendChild(container);
                ProcessJsonArray((JArray)property.Value, container, doc, currentPath, nonLocalizableFields);
            }
            else if (property.Value?.Type == JTokenType.String)
            {
                if (IsHtmlConvent(property.Value.ToString()))
                {
                    var content = property.Value.ToString();
                    var valueDiv = doc.CreateElement("div");
                    valueDiv.SetAttributeValue("class", "property-value");
                    valueDiv.SetAttributeValue("data-json-path", currentPath);
                    valueDiv.SetAttributeValue("data-html", "true");
                    valueDiv.InnerHtml = content;
                    parentNode.AppendChild(valueDiv);
                }
                else
                {
                    var container = MarkdownConverter.ToHtml(doc, currentPath, property.Value);
                    parentNode.AppendChild(container);
                }
            }
        }
    }

    private static bool IsHtmlConvent(string content)
    {
        return content.Contains("<p") && content.Contains("/p>") ||
               content.Contains("<div") && content.Contains("/div>") ||
               content.Contains("<span") && content.Contains("/span>") ||
               content.Contains("<strong") && content.Contains("/strong>") ||
               content.Contains("<em") && content.Contains("/em>") ||
               content.Contains("<u") && content.Contains("/u>");
    }

    private static void ProcessJsonArray(JArray array, HtmlNode parentNode, HtmlDocument doc, string jsonPath, IEnumerable<string>? nonLocalizableFields)
    {
        if (IsRichTextEditorData(array))
        {
            ProcessRichTextEditorContent(array, parentNode, doc, jsonPath);
            return;
        }

        for (int i = 0; i < array.Count; i++)
        {
            var item = array[i];
            string itemPath = $"{jsonPath}[{i}]";

            var itemContainer = CreateContainerElement(doc, "div", "array-item", itemPath);
            parentNode.AppendChild(itemContainer);

            if (item.Type == JTokenType.Object)
            {
                ProcessJsonObject((JObject)item, itemContainer, doc, itemPath, nonLocalizableFields);
            }
            else if (item.Type == JTokenType.Array)
            {
                ProcessJsonArray((JArray)item, itemContainer, doc, itemPath, nonLocalizableFields);
            }
            else if (item.Type == JTokenType.String)
            {
                var valueNode = CreateElement(doc, "span");
                valueNode.SetAttributeValue("data-json-path", itemPath);
                valueNode.InnerHtml = item.ToString();
                itemContainer.AppendChild(valueNode);
            }
        }
    }

    private static bool IsRichTextEditorData(JArray array)
    {
        if (array.Count == 0) return false;

        return array.All(item =>
            item.Type == JTokenType.Object &&
            ((JObject)item)["type"] != null &&
            ((JObject)item)["children"] != null);
    }

    private static void ProcessRichTextEditorContent(JArray array, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
    {
        var richTextContainer = CreateContainerElement(doc, "div", "rich-text-content", jsonPath);

        for (int i = 0; i < array.Count; i++)
        {
            var block = (JObject)array[i];
            string blockPath = $"{jsonPath}[{i}]";
            string blockType = block["type"]?.ToString() ?? "paragraph";

            HtmlNode blockElement = CreateRichTextBlockElement(doc, blockType, block);
            blockElement.SetAttributeValue("data-json-path", blockPath);

            var children = block["children"] as JArray;
            if (children != null)
            {
                ProcessRichTextChildren(children, blockElement, doc, blockPath);
            }

            richTextContainer.AppendChild(blockElement);
        }

        parentNode.AppendChild(richTextContainer);
    }

    private static HtmlNode CreateRichTextBlockElement(HtmlDocument doc, string blockType, JObject blockData)
    {
        switch (blockType)
        {
            case "heading":
                int level = int.Parse(blockData["level"]?.ToString() ?? "1");
                return CreateElement(doc, "h" + Math.Min(Math.Max(level, 1), 6));

            case "list":
                return CreateElement(doc, blockData["format"]?.ToString() == "ordered" ? "ol" : "ul");

            case "list-item":
                return CreateElement(doc, "li");

            case "code":
                var preElement = CreateElement(doc, "pre");
                var codeElement = CreateElement(doc, "code");
                preElement.AppendChild(codeElement);
                return preElement;

            case "quote":
                return CreateElement(doc, "blockquote");

            case "link":
                var linkElement = CreateElement(doc, "a");
                if (blockData["url"] != null)
                {
                    linkElement.SetAttributeValue("href", blockData["url"]?.ToString() ?? "#");
                }
                return linkElement;

            default:
                return CreateElement(doc, "p");
        }
    }

    private static void ProcessRichTextChildren(JArray children, HtmlNode parentNode, HtmlDocument doc, string parentPath)
    {
        for (int j = 0; j < children.Count; j++)
        {
            var child = children[j] as JObject;
            if (child == null) continue;

            string childPath = $"{parentPath}.children[{j}]";
            string childType = child["type"]?.ToString() ?? "";

            if (childType == "text")
            {
                ProcessTextNode(child, parentNode, doc, childPath);
            }
            else if (childType == "list-item")
            {
                ProcessListItemNode(child, parentNode, doc, childPath);
            }
            else
            {
                ProcessNestedBlockNode(child, parentNode, doc, childPath, childType);
            }
        }
    }

    private static void ProcessTextNode(JObject textNode, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
    {
        string text = textNode["text"]?.ToString() ?? "";
        bool isBold = textNode["bold"]?.ToObject<bool>() ?? false;
        bool isItalic = textNode["italic"]?.ToObject<bool>() ?? false;
        bool isUnderline = textNode["underline"]?.ToObject<bool>() ?? false;

        HtmlNode node = CreateTextFormattingNode(doc, text, isBold, isItalic, isUnderline);
        node.SetAttributeValue("data-json-path", $"{jsonPath}.text");
        parentNode.AppendChild(node);
    }

    private static void ProcessListItemNode(JObject listItemNode, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
    {
        var listItem = CreateElement(doc, "li");
        listItem.SetAttributeValue("data-json-path", jsonPath);

        var children = listItemNode["children"] as JArray;
        if (children != null)
        {
            ProcessRichTextChildren(children, listItem, doc, jsonPath);
        }

        parentNode.AppendChild(listItem);
    }

    private static void ProcessNestedBlockNode(JObject blockNode, HtmlNode parentNode, HtmlDocument doc, string jsonPath, string blockType)
    {
        var children = blockNode["children"] as JArray;
        if (children != null)
        {
            HtmlNode nestedElement = CreateRichTextBlockElement(doc, blockType, blockNode);
            nestedElement.SetAttributeValue("data-json-path", jsonPath);
            ProcessRichTextChildren(children, nestedElement, doc, jsonPath);
            parentNode.AppendChild(nestedElement);
        }
    }

    private static HtmlNode CreateTextFormattingNode(HtmlDocument doc, string text, bool isBold, bool isItalic, bool isUnderline)
    {
        HtmlNode textNode = doc.CreateTextNode(text);
        HtmlNode formattedNode;

        if (isBold)
        {
            formattedNode = CreateElement(doc, "strong");
        }
        else if (isItalic)
        {
            formattedNode = CreateElement(doc, "em");
        }
        else if (isUnderline)
        {
            formattedNode = CreateElement(doc, "u");
        }
        else
        {
            formattedNode = CreateElement(doc, "span");
        }

        formattedNode.AppendChild(textNode);
        return formattedNode;
    }

    private static HtmlNode CreateElement(HtmlDocument doc, string tagName)
    {
        return doc.CreateElement(tagName);
    }

    private static HtmlNode CreateContainerElement(HtmlDocument doc, string tagName, string className, string jsonPath)
    {
        var container = CreateElement(doc, tagName);
        container.SetAttributeValue("class", className);
        container.SetAttributeValue("data-json-path", jsonPath);
        return container;
    }

    private static string AppendJsonPath(string basePath, string propertyName)
    {
        return string.IsNullOrEmpty(basePath) ? propertyName : $"{basePath}.{propertyName}";
    }
}
