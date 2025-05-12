using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Utils.Converters;

public static class MarkdownConverter
{
    public static HtmlNode ToHtml(HtmlDocument doc, string currentPath, JToken jToken)
    {
        string value = jToken.ToString();

        var container = doc.CreateElement("div");

        if (value.Contains("\n"))
        {
            var lines = value.Split(['\n']);
            container.SetAttributeValue("class", "md-rich-text");
            container.SetAttributeValue("data-json-path", currentPath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    var br = doc.CreateElement("br");
                    container.AppendChild(br);
                    continue;
                }

                var p = doc.CreateElement("p");
                p.SetAttributeValue("class", "property-value");
                p.InnerHtml = line;
                container.AppendChild(p);
            }
        }
        else
        {
            var valueSpan = doc.CreateElement("span");
            valueSpan.SetAttributeValue("class", "property-value");
            valueSpan.SetAttributeValue("data-json-path", currentPath);
            valueSpan.InnerHtml = value;
            container.AppendChild(valueSpan);
        }

        return container;
    }

    public static string ToMarkdown(HtmlNode node)
    {
        var sb = new StringBuilder();

        foreach (var child in node.ChildNodes)
        {
            if (child.Name == "p")
            {
                sb.AppendLine(child.InnerText);
            }
            else if (child.Name == "br")
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(child.InnerText);
            }
        }

        return sb.ToString();
    }
}
