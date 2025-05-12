using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Strapi.Utils.Converters;

public static class RichTextConverter
{
    public static JObject BuildBlock(HtmlNode blockNode)
    {
        var blockJson = new JObject();
        string blockType = GetBlockType(blockNode);
        blockJson["type"] = blockType;

        AddBlockTypeSpecificProperties(blockJson, blockNode, blockType);
        
        var children = blockType == "list" 
            ? ProcessListItems(blockNode) 
            : ProcessTextNodes(blockNode);
            
        blockJson["children"] = children;
        return blockJson;
    }
    
    private static void AddBlockTypeSpecificProperties(JObject blockJson, HtmlNode blockNode, string blockType)
    {
        switch (blockType)
        {
            case "heading":
                int level = int.Parse(blockNode.Name.Substring(1));
                blockJson["level"] = level;
                break;
            case "list":
                blockJson["format"] = blockNode.Name == "ul" ? "unordered" : "ordered";
                break;
        }
    }
    
    private static JArray ProcessListItems(HtmlNode blockNode)
    {
        var children = new JArray();
        
        foreach (var listItemNode in blockNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
        {
            var listItemJson = new JObject
            {
                ["type"] = "list-item",
                ["children"] = ProcessTextNodes(listItemNode)
            };
            
            children.Add(listItemJson);
        }
        
        return children;
    }
    
    private static JArray ProcessTextNodes(HtmlNode parentNode)
    {
        var textNodes = new JArray();
        
        foreach (var node in parentNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
        {
            var textJson = new JObject
            {
                ["type"] = "text",
                ["text"] = node.InnerText
            };
            
            ApplyTextFormatting(textJson, node);
            textNodes.Add(textJson);
        }
        
        return textNodes;
    }
    
    private static void ApplyTextFormatting(JObject textJson, HtmlNode node)
    {
        if (HasStyle(node, "strong"))
            textJson["bold"] = true;
            
        if (HasStyle(node, "em"))
            textJson["italic"] = true;
            
        if (HasStyle(node, "u"))
            textJson["underline"] = true;
    }
    
    private static bool HasStyle(HtmlNode node, string style)
    {
        return node.Name == style || node.ParentNode.Name == style;
    }
    
    private static string GetBlockType(HtmlNode node) => node.Name switch
    {
        var name when name.StartsWith("h") => "heading",
        "ol" or "ul" => "list",
        "li" => "list-item",
        "pre" => "code",
        "blockquote" => "quote",
        _ => "paragraph",
    };
}