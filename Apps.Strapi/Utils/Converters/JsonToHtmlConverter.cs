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

    public static string ConvertToHtml(string json, string contentId, string contentType)
    {
        var jsonObj = JsonConvert.DeserializeObject<JObject>(json)!;
        var dataObj = jsonObj["data"] as JObject;

        if (dataObj == null)
        {
            throw new ArgumentException("Invalid JSON structure. Expected 'data' property.");
        }

        var doc = new HtmlDocument();

        var htmlNode = doc.CreateElement("html");
        doc.DocumentNode.AppendChild(htmlNode);

        var headNode = doc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var metaCharset = doc.CreateElement("meta");
        metaCharset.SetAttributeValue("charset", "UTF-8");
        headNode.AppendChild(metaCharset);

        var metaContentId = doc.CreateElement("meta");
        metaContentId.SetAttributeValue("name", MetadataKeys.ContentId);
        metaContentId.SetAttributeValue("content", contentId);
        headNode.AppendChild(metaContentId);

        var metaContentType = doc.CreateElement("meta");
        metaContentType.SetAttributeValue("name", MetadataKeys.ContentType);
        metaContentType.SetAttributeValue("content", contentType);
        headNode.AppendChild(metaContentType);

        string locale = dataObj["locale"]?.ToString() ?? "en";
        var metaLocale = doc.CreateElement("meta");
        metaLocale.SetAttributeValue("name", MetadataKeys.Locale);
        metaLocale.SetAttributeValue("content", locale);
        headNode.AppendChild(metaLocale);

        var bodyNode = doc.CreateElement("body");
        bodyNode.SetAttributeValue("original", HttpUtility.HtmlEncode(jsonObj.ToString(Formatting.None)));
        htmlNode.AppendChild(bodyNode);

        ProcessJsonObject(dataObj, bodyNode, doc, "data");

        return "<!DOCTYPE html>\n" + doc.DocumentNode.OuterHtml;
    }

    private static void ProcessJsonObject(JObject jsonObj, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
    {
        foreach (var property in jsonObj)
        {
            string currentPath = AppendJsonPath(jsonPath, property.Key);

            if (JsonProperties.NonLocalizableProperties.Contains(property.Key))
            {
                continue;
            }

            if (property.Value?.Type == JTokenType.Object)
            {
                var container = doc.CreateElement("div");
                container.SetAttributeValue("class", "json-object");
                container.SetAttributeValue("data-json-path", currentPath);

                parentNode.AppendChild(container);
                ProcessJsonObject((JObject)property.Value, container, doc, currentPath);
            }
            else if (property.Value?.Type == JTokenType.Array)
            {
                var container = doc.CreateElement("div");
                container.SetAttributeValue("class", "json-array");
                container.SetAttributeValue("data-json-path", currentPath);

                parentNode.AppendChild(container);
                ProcessJsonArray((JArray)property.Value, container, doc, currentPath);
            }
            else if (property.Value?.Type == JTokenType.String)
            {
                var container = MarkdownConverter.ToHtml(doc, currentPath, property.Value);
                parentNode.AppendChild(container);
            }
        }
    }

    private static void ProcessJsonArray(JArray array, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
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

            var itemContainer = doc.CreateElement("div");
            itemContainer.SetAttributeValue("class", "array-item");
            itemContainer.SetAttributeValue("data-json-path", itemPath);
            parentNode.AppendChild(itemContainer);

            if (item.Type == JTokenType.Object)
            {
                ProcessJsonObject((JObject)item, itemContainer, doc, itemPath);
            }
            else if (item.Type == JTokenType.Array)
            {
                ProcessJsonArray((JArray)item, itemContainer, doc, itemPath);
            }
            else if (item.Type == JTokenType.String)
            {
                var valueNode = doc.CreateElement("span");
                valueNode.SetAttributeValue("data-json-path", itemPath);
                valueNode.InnerHtml = item.ToString();
                itemContainer.AppendChild(valueNode);
            }
        }
    }

    private static bool IsRichTextEditorData(JArray array)
    {
        if (array.Count == 0) return false;

        foreach (var item in array)
        {
            if (item.Type != JTokenType.Object) return false;

            var obj = (JObject)item;
            if (obj["type"] == null || obj["children"] == null) return false;
        }

        return true;
    }

    private static void ProcessRichTextEditorContent(JArray array, HtmlNode parentNode, HtmlDocument doc, string jsonPath)
    {
        var richTextContainer = doc.CreateElement("div");
        richTextContainer.SetAttributeValue("class", "rich-text-content");
        richTextContainer.SetAttributeValue("data-json-path", jsonPath);

        for (int i = 0; i < array.Count; i++)
        {
            var block = (JObject)array[i];
            string blockPath = $"{jsonPath}[{i}]";
            string blockType = block["type"]?.ToString() ?? "paragraph";

            // Create the appropriate block element
            HtmlNode blockElement;
            switch (blockType)
            {
                case "heading":
                    int level = int.Parse(block["level"]?.ToString() ?? "1");
                    blockElement = doc.CreateElement("h" + Math.Min(Math.Max(level, 1), 6));
                    break;
                case "list":
                    blockElement = doc.CreateElement(block["format"]?.ToString() == "ordered" ? "ol" : "ul");
                    break;
                case "list-item":
                    blockElement = doc.CreateElement("li");
                    break;
                case "code":
                    blockElement = doc.CreateElement("pre");
                    var codeBlock = doc.CreateElement("code");
                    blockElement.AppendChild(codeBlock);
                    blockElement = codeBlock;
                    break;
                case "quote":
                    blockElement = doc.CreateElement("blockquote");
                    break;
                default:
                    blockElement = doc.CreateElement("p");
                    break;
            }

            blockElement.SetAttributeValue("data-json-path", blockPath);

            // Process the children of this block
            var children = block["children"] as JArray;
            if (children != null)
            {
                ProcessRichTextChildren(children, blockElement, doc, blockPath);
            }

            richTextContainer.AppendChild(blockElement);
        }

        parentNode.AppendChild(richTextContainer);
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
                string text = child["text"]?.ToString() ?? "";
                bool isBold = child["bold"]?.ToObject<bool>() ?? false;
                bool isItalic = child["italic"]?.ToObject<bool>() ?? false;
                bool isUnderline = child["underline"]?.ToObject<bool>() ?? false;

                HtmlNode textNode = doc.CreateTextNode(text);
                HtmlNode formattedNode;

                if (isBold)
                {
                    formattedNode = doc.CreateElement("strong");
                }
                else if (isItalic)
                {
                    formattedNode = doc.CreateElement("em");
                }
                else if (isUnderline)
                {
                    formattedNode = doc.CreateElement("u");
                }
                else
                {
                    formattedNode = doc.CreateElement("span");
                }

                formattedNode.SetAttributeValue("data-json-path", $"{childPath}.text");
                formattedNode.AppendChild(textNode);
                parentNode.AppendChild(formattedNode);
            }
            else if (childType == "list-item")
            {
                // For list items found as children of lists
                var listItemNode = doc.CreateElement("li");
                listItemNode.SetAttributeValue("data-json-path", childPath);

                // Process the children of this list item
                var listItemChildren = child["children"] as JArray;
                if (listItemChildren != null)
                {
                    ProcessRichTextChildren(listItemChildren, listItemNode, doc, childPath);
                }

                parentNode.AppendChild(listItemNode);
            }
            else
            {
                // Handle other potential block types recursively
                var nestedChildren = child["children"] as JArray;
                if (nestedChildren != null)
                {
                    // Create appropriate element based on child type
                    HtmlNode nestedElement;
                    switch (childType)
                    {
                        case "heading":
                            int level = int.Parse(child["level"]?.ToString() ?? "1");
                            nestedElement = doc.CreateElement("h" + Math.Min(Math.Max(level, 1), 6));
                            break;
                        case "list":
                            nestedElement = doc.CreateElement(child["format"]?.ToString() == "ordered" ? "ol" : "ul");
                            break;
                        case "code":
                            nestedElement = doc.CreateElement("pre");
                            var codeBlock = doc.CreateElement("code");
                            nestedElement.AppendChild(codeBlock);
                            nestedElement = codeBlock;
                            break;
                        case "quote":
                            nestedElement = doc.CreateElement("blockquote");
                            break;
                        default:
                            nestedElement = doc.CreateElement("div");
                            break;
                    }

                    nestedElement.SetAttributeValue("data-json-path", childPath);
                    ProcessRichTextChildren(nestedChildren, nestedElement, doc, childPath);
                    parentNode.AppendChild(nestedElement);
                }
            }
        }
    }

    private static string AppendJsonPath(string basePath, string propertyName)
    {
        return string.IsNullOrEmpty(basePath) ? propertyName : $"{basePath}.{propertyName}";
    }
}
