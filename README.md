# CliOptions

A simple and easy to use .NET library for parsing arguments in a command-line application.

### Code example:

```csharp
using System;
using CliOptions;
using CliOptions.Options;

public static class Program
{
    public static void Main(string[] args)
    {
        var myOptions = new MyOptions();
        myOptions.Parse(args);

        Console.WriteLine(myOptions.PrintText);
    }
}

public class MyOptions : Parser
{
    [Option("help", 'h', "Displays all available options.")]
    public void Help()
    {
        Console.WriteLine("Application options:\n" + this.HelpText);

        // Application options:
        //     -h, --help                    Displays all available options.
        //     --print-text [TEXT]           Sets the output
    }

    [Option("print-text", description: "Sets the output", valueName: "TEXT")]
    public string PrintText { get; set; } = "No text specified.";
}
```