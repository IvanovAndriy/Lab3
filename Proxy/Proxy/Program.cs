using System;
using System.IO;
using System.Text.RegularExpressions;

public interface ISmartTextReader
{
    char[][] ReadText(string filePath);
}

public class SmartTextReader : ISmartTextReader
{
    public char[][] ReadText(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            char[][] result = new char[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].ToCharArray();
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }
}

public class SmartTextChecker : ISmartTextReader
{
    private readonly ISmartTextReader reader;

    public SmartTextChecker(ISmartTextReader reader)
    {
        this.reader = reader;
    }

    public char[][] ReadText(string filePath)
    {
        Console.WriteLine($"Attempting to open file: {filePath}");

        char[][] result = reader.ReadText(filePath);

        if (result != null)
        {
            Console.WriteLine($"File successfully opened and read");

            int lineCount = result.Length;
            int charCount = 0;
            foreach (var line in result)
            {
                charCount += line.Length;
            }

            Console.WriteLine($"Total lines: {lineCount}");
            Console.WriteLine($"Total characters: {charCount}");
            Console.WriteLine($"File closed");
        }
        else
        {
            Console.WriteLine("Failed to read file");
        }

        return result;
    }
}

public class SmartTextReaderLocker : ISmartTextReader
{
    private readonly ISmartTextReader reader;
    private readonly string restrictedPattern;

    public SmartTextReaderLocker(ISmartTextReader reader, string restrictedPattern)
    {
        this.reader = reader;
        this.restrictedPattern = restrictedPattern;
    }

    public char[][] ReadText(string filePath)
    {
        if (Regex.IsMatch(filePath, restrictedPattern))
        {
            Console.WriteLine($"Access denied! File {filePath} is restricted.");
            return null;
        }

        return reader.ReadText(filePath);
    }
}

class Program
{
    static void Main(string[] args)
    {
        string testFilePath = "test.txt";
        string restrictedFilePath = "secret.txt";

        File.WriteAllText(testFilePath, "Hello\nWorld\nThis is a test");
        File.WriteAllText(restrictedFilePath, "Secret data");

        Console.WriteLine("=== Testing SmartTextReader ===");
        ISmartTextReader reader = new SmartTextReader();
        char[][] result = reader.ReadText(testFilePath);
        if (result != null)
        {
            Console.WriteLine("File contents:");
            foreach (var line in result)
            {
                Console.WriteLine(new string(line));
            }
        }

        Console.WriteLine("\n=== Testing SmartTextChecker ===");
        ISmartTextReader checker = new SmartTextChecker(new SmartTextReader());
        checker.ReadText(testFilePath);

        Console.WriteLine("\n=== Testing SmartTextReaderLocker ===");
        ISmartTextReader locker = new SmartTextReaderLocker(
            new SmartTextReader(),
            @"secret\.txt$"
        );

        Console.WriteLine("\nTrying to read allowed file:");
        locker.ReadText(testFilePath);

        Console.WriteLine("\nTrying to read restricted file:");
        locker.ReadText(restrictedFilePath);

        File.Delete(testFilePath);
        File.Delete(restrictedFilePath);
    }
}