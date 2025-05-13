using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Utils.Converters;

public static class MarkdownConverter
{
    public static HtmlNode ToHtml(HtmlDocument htmlDocument, string jsonPath, JToken markdownContent)
    {
        if (htmlDocument == null) throw new ArgumentNullException(nameof(htmlDocument));
        if (markdownContent == null) throw new ArgumentNullException(nameof(markdownContent));

        string contentValue = markdownContent.ToString();
        var containerDiv = htmlDocument.CreateElement("div");

        if (contentValue.Contains('\n'))
        {
            RenderMultilineContent(htmlDocument, jsonPath, contentValue, containerDiv);
        }
        else
        {
            RenderSingleLineContent(htmlDocument, jsonPath, contentValue, containerDiv);
        }

        return containerDiv;
    }

    public static string ToMarkdown(HtmlNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        var markdownBuilder = new StringBuilder();

        foreach (var child in node.ChildNodes)
        {
            if (child.Name == "p")
            {
                markdownBuilder.AppendLine(child.InnerText);
            }
            else if (child.Name == "br")
            {
                markdownBuilder.AppendLine();
            }
            else
            {
                markdownBuilder.Append(child.InnerText);
            }
        }

        return markdownBuilder.ToString();
    }

    #region Private Helper Methods

    private static void RenderMultilineContent(HtmlDocument htmlDocument, string jsonPath, string content, HtmlNode containerDiv)
    {
        string[] lines = content.Split(['\n']);

        SetContainerAttributes(containerDiv, jsonPath, isRichText: true);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                var lineBreak = htmlDocument.CreateElement("br");
                containerDiv.AppendChild(lineBreak);
                continue;
            }

            var paragraph = htmlDocument.CreateElement("p");
            paragraph.SetAttributeValue("class", "property-value");
            paragraph.InnerHtml = line;
            containerDiv.AppendChild(paragraph);
        }
    }

    private static void RenderSingleLineContent(HtmlDocument htmlDocument, string jsonPath, string content, HtmlNode containerDiv)
    {
        var valueSpan = htmlDocument.CreateElement("span");
        valueSpan.SetAttributeValue("class", "property-value");
        valueSpan.SetAttributeValue("data-json-path", jsonPath);
        valueSpan.InnerHtml = content;
        containerDiv.AppendChild(valueSpan);
    }

    private static void SetContainerAttributes(HtmlNode container, string jsonPath, bool isRichText)
    {
        if (isRichText)
        {
            container.SetAttributeValue("class", "md-rich-text");
        }

        container.SetAttributeValue("data-json-path", jsonPath);
    }

    #endregion
}