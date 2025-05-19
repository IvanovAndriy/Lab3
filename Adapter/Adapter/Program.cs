using System;
using System.IO;

public class Logger
{
    public void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[LOG]: " + message);
        Console.ResetColor();
    }

    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[ERROR]: " + message);
        Console.ResetColor();
    }

    public void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[WARNING]: " + message);
        Console.ResetColor();
    }
}

public class FileWriter
{
    private readonly string _filePath;

    public FileWriter(string filePath)
    {
        _filePath = filePath;
    }

    public void Write(string message)
    {
        File.AppendAllText(_filePath, message);
    }

    public void WriteLine(string message)
    {
        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}
public class FileLoggerAdapter
{
    private readonly FileWriter _fileWriter;

    public FileLoggerAdapter(FileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public void Log(string message)
    {
        _fileWriter.WriteLine("[LOG]: " + message);
    }

    public void Error(string message)
    {
        _fileWriter.WriteLine("[ERROR]: " + message);
    }

    public void Warn(string message)
    {
        _fileWriter.WriteLine("[WARNING]: " + message);
    }
}
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Console Logger ===");
        Logger consoleLogger = new Logger();
        consoleLogger.Log("System started successfully.");
        consoleLogger.Warn("Low disk space.");
        consoleLogger.Error("Failed to connect to database.");

        Console.WriteLine("\n=== File Logger ===");
        FileWriter writer = new FileWriter("log.txt");
        FileLoggerAdapter fileLogger = new FileLoggerAdapter(writer);
        fileLogger.Log("System started successfully.");
        fileLogger.Warn("Low disk space.");
        fileLogger.Error("Failed to connect to database.");

        Console.WriteLine("Messages were written to log.txt.");
    }
}
