# CliOptions

A simple and easy to use .NET library for parsing arguments in a command-line application.

### Code example:

```csharp
using System;
using CliOptions;
public static class Program
{
    public static void Main(string[] args)
    {
        var myOptions = new MyOptions();
        myOptions.Parse(args);

        Console.WriteLine("Verbose mode is set to: " + myOptions.IsVerboseOn);
    }
}

public class MyOptions : Parser
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