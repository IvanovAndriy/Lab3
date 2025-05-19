using System;

public interface IRenderer
{
    void RenderShape(string shapeName);
}

public class VectorRenderer : IRenderer
{
    public void RenderShape(string shapeName)
    {
        Console.WriteLine($"Drawing {shapeName} as vectors");
    }
}

public class RasterRenderer : IRenderer
{
    public void RenderShape(string shapeName)
    {
        Console.WriteLine($"Drawing {shapeName} as pixels");
    }
}

public abstract class Shape
{
    protected IRenderer renderer;
    protected string name;

    protected Shape(IRenderer renderer, string name)
    {
        this.renderer = renderer;
        this.name = name;
    }

    public void Draw()
    {
        renderer.RenderShape(name);
    }
}

public class Circle : Shape
{
    public Circle(IRenderer renderer) : base(renderer, "Circle") { }
}

public class Square : Shape
{
    public Square(IRenderer renderer) : base(renderer, "Square") { }
}

public class Triangle : Shape
{
    public Triangle(IRenderer renderer) : base(renderer, "Triangle") { }
}

class Program
{
    static void Main(string[] args)
    {
        IRenderer vectorRenderer = new VectorRenderer();
        IRenderer rasterRenderer = new RasterRenderer();

        Shape circleVector = new Circle(vectorRenderer);
        Shape circleRaster = new Circle(rasterRenderer);
        Shape squareVector = new Square(vectorRenderer);
        Shape squareRaster = new Square(rasterRenderer);
        Shape triangleVector = new Triangle(vectorRenderer);
        Shape triangleRaster = new Triangle(rasterRenderer);

        Console.WriteLine("Rendering shapes:");
        circleVector.Draw();
        circleRaster.Draw();
        squareVector.Draw();
        squareRaster.Draw();
        triangleVector.Draw();
        triangleRaster.Draw();
    }
}