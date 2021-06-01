# CliOptions

A simple and easy to use .NET library for parsing arguments in a command-line application.

## Usage

Download from NuGet: `dotnet add package CliOptions`

### Code example:

```csharp
using System;
using CliOptions;

public static class Program
{
    private Options _options = new();

    public static void Main(string[] args)
    {
        _options.Parse(args);
        Console.WriteLine("Verbose mode is set to: " + myOptions.IsVerboseOn);
    }
}

public class Options : ArgumentsParser
{
    [PropertyOption("verbose", 'v', "Sets whether to be verbose.")]
    public bool IsVerboseOn { get; set; } = false;

    [MethodOption("help", 'h', "Displays all available options.")]
    public void Help()
    {
        Console.WriteLine("Application options:\n" + this.HelpText);

        // Application options:
        //    -v, --verbose [BOOLEAN]               Sets whether to be verbose.
        //    -h, --help                            Displays all available options.
        //    --output-text [STRING] [INT32]        Output some text.
    }

    [MethodOption("output-text", description: "Output some text.")]
    public void OutputText(string textToOutput, int numberOfTimes)
    {
        for (int i = 0; i < numberOfTimes; i++)
            Console.WriteLine(textToOutput);
    }
}
```