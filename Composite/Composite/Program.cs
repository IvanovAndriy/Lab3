using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class LightNode
{
    public abstract string OuterHTML { get; }
    public abstract string InnerHTML { get; }
}

public class LightTextNode : LightNode
{
    private readonly string text;

    public LightTextNode(string text)
    {
        this.text = text;
    }

    public override string OuterHTML => text;
    public override string InnerHTML => text;
}

public enum DisplayType
{
    Block,
    Inline
}

public enum ClosingType
{
    Single,
    Paired
}

public class LightElementNode : LightNode
{
    private readonly string tagName;
    private readonly DisplayType displayType;
    private readonly ClosingType closingType;
    private readonly List<string> cssClasses;
    private readonly List<LightNode> children;

    public LightElementNode(string tagName, DisplayType displayType, ClosingType closingType, List<string> cssClasses = null)
    {
        this.tagName = tagName;
        this.displayType = displayType;
        this.closingType = closingType;
        this.cssClasses = cssClasses ?? new List<string>();
        this.children = new List<LightNode>();
    }

    public void AddChild(LightNode node)
    {
        children.Add(node);
    }

    public int ChildCount => children.Count;

    public override string OuterHTML
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"<{tagName}");
            if (cssClasses.Any())
            {
                sb.Append($" class=\"{string.Join(" ", cssClasses)}\"");
            }
            sb.Append(">");

            if (closingType == ClosingType.Paired)
            {
                sb.Append(InnerHTML);
                sb.Append($"</{tagName}>");
            }

            return sb.ToString();
        }
    }

    public override string InnerHTML
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            foreach (var child in children)
            {
                sb.Append(child.OuterHTML);
            }
            return sb.ToString();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var ul = new LightElementNode("ul", DisplayType.Block, ClosingType.Paired, new List<string> { "nav-list", "main-menu" });

        var items = new[] { "Home", "About", "Services", "Contact" };
        foreach (var item in items)
        {
            var li = new LightElementNode("li", DisplayType.Block, ClosingType.Paired);
            li.AddChild(new LightTextNode(item));
            ul.AddChild(li);
        }
        Console.WriteLine(ul.OuterHTML);

    }
}