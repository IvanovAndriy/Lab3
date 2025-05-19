using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

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

public class ElementIntrinsicState
{
    public string TagName { get; }
    public DisplayType DisplayType { get; }
    public ClosingType ClosingType { get; }
    public List<string> CssClasses { get; }

    public ElementIntrinsicState(string tagName, DisplayType displayType, ClosingType closingType, List<string> cssClasses)
    {
        TagName = tagName;
        DisplayType = displayType;
        ClosingType = closingType;
        CssClasses = cssClasses ?? new List<string>();
    }
}

public class LightElementNodeFactory
{
    private readonly Dictionary<string, ElementIntrinsicState> _elementStates = new Dictionary<string, ElementIntrinsicState>();
    public int CreatedStatesCount => _elementStates.Count; 

    public ElementIntrinsicState GetElementState(string tagName, DisplayType displayType, ClosingType closingType, List<string> cssClasses = null)
    {
        string key = $"{tagName}_{(int)displayType}_{(int)closingType}";
        if (cssClasses != null && cssClasses.Any())
        {
            key += $"_{string.Join(",", cssClasses)}";
        }

        if (!_elementStates.ContainsKey(key))
        {
            _elementStates[key] = new ElementIntrinsicState(tagName, displayType, closingType, cssClasses);
        }
        return _elementStates[key];
    }
}

public class LightElementNode : LightNode
{
    private readonly ElementIntrinsicState intrinsicState;
    private readonly List<LightNode> children;

    public LightElementNode(ElementIntrinsicState intrinsicState)
    {
        this.intrinsicState = intrinsicState;
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
            sb.Append($"<{intrinsicState.TagName}");
            if (intrinsicState.CssClasses.Any())
            {
                sb.Append($" class=\"{string.Join(" ", intrinsicState.CssClasses)}\"");
            }
            sb.Append(">");

            if (intrinsicState.ClosingType == ClosingType.Paired)
            {
                sb.Append(InnerHTML);
                sb.Append($"</{intrinsicState.TagName}>");
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

public class BookConverter
{
    private readonly LightElementNodeFactory factory;

    public BookConverter(LightElementNodeFactory factory)
    {
        this.factory = factory;
    }

    public LightElementNode ConvertToHTMLFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            var root = new LightElementNode(factory.GetElementState("div", DisplayType.Block, ClosingType.Paired, new List<string> { "book" }));

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd();
                LightElementNode element;

                if (i == 0)
                {
                    element = new LightElementNode(factory.GetElementState("h1", DisplayType.Block, ClosingType.Paired));
                }
                else if (line.Length < 20)
                {
                    element = new LightElementNode(factory.GetElementState("h2", DisplayType.Block, ClosingType.Paired));
                }
                else if (line.StartsWith(" "))
                {
                    element = new LightElementNode(factory.GetElementState("blockquote", DisplayType.Block, ClosingType.Paired));
                }
                else
                {
                    element = new LightElementNode(factory.GetElementState("p", DisplayType.Block, ClosingType.Paired));
                }

                element.AddChild(new LightTextNode(line));
                root.AddChild(element);
            }

            return root;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
            return null;
        }
    }

    public LightElementNode ConvertToHTMLWithoutFlyweightFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            var root = new LightElementNode(new ElementIntrinsicState("div", DisplayType.Block, ClosingType.Paired, new List<string> { "book" }));

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd();
                LightElementNode element;

                if (i == 0)
                {
                    element = new LightElementNode(new ElementIntrinsicState("h1", DisplayType.Block, ClosingType.Paired, null));
                }
                else if (line.Length < 20)
                {
                    element = new LightElementNode(new ElementIntrinsicState("h2", DisplayType.Block, ClosingType.Paired, null));
                }
                else if (line.StartsWith(" "))
                {
                    element = new LightElementNode(new ElementIntrinsicState("blockquote", DisplayType.Block, ClosingType.Paired, null));
                }
                else
                {
                    element = new LightElementNode(new ElementIntrinsicState("p", DisplayType.Block, ClosingType.Paired, null));
                }

                element.AddChild(new LightTextNode(line));
                root.AddChild(element);
            }

            return root;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
            return null;
        }
    }
}

class Program
{
    static long GetMemoryUsage()
    {
        for (int i = 0; i < 3; i++)
        {
            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            Thread.Sleep(200); 
        }
        return Process.GetCurrentProcess().PrivateMemorySize64;
    }

    static void Main(string[] args)
    {
        string filePath = "book.txt";

        int lineCount = File.ReadAllLines(filePath).Length;

        var factory = new LightElementNodeFactory();
        var converter = new BookConverter(factory);

        long memoryBeforeFlyweight = GetMemoryUsage();
        var htmlRootFlyweight = converter.ConvertToHTMLFromFile(filePath);
        long memoryAfterFlyweight = GetMemoryUsage();

        long memoryBeforeNoFlyweight = GetMemoryUsage();
        var htmlRootNoFlyweight = converter.ConvertToHTMLWithoutFlyweightFromFile(filePath);
        long memoryAfterNoFlyweight = GetMemoryUsage();

        Console.WriteLine("\nGenerated HTML (with Flyweight):");
        Console.WriteLine(htmlRootFlyweight.OuterHTML);
        Console.WriteLine($"\nNumber of child elements in root: {htmlRootFlyweight.ChildCount}");
        Console.WriteLine($"Memory with Flyweight: {(memoryAfterFlyweight - memoryBeforeFlyweight) / 1024} KB");
        Console.WriteLine($"Number of unique states (Flyweight): {factory.CreatedStatesCount}");

        Console.WriteLine("\nGenerated HTML (without Flyweight):");
        Console.WriteLine(htmlRootNoFlyweight.OuterHTML);
        Console.WriteLine($"\nNumber of child elements in root: {htmlRootNoFlyweight.ChildCount}");
        Console.WriteLine($"Memory without Flyweight: {(memoryAfterNoFlyweight - memoryBeforeNoFlyweight) / 1024} KB");
        Console.WriteLine($"Number of created states (without Flyweight): {htmlRootNoFlyweight.ChildCount + 1}");
    }
}