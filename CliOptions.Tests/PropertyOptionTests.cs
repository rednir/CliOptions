using System;
using Xunit;
using CliOptions;
using CliOptions.Exceptions;

namespace CliOptions.Tests
{
    public class PropertyOptionTests
    {
        [Fact]
        public void BuiltInTypesTest()
        {
            string[] args = new[]
            {
                "--bool", "truE",
                "--string", "Hello",
                "--int", "5000",
                "--dynamic", "Bye",
            };

            BuiltInTypesParser parser = new();
            parser.Parse(args);

            Assert.True(parser.ShouldBeTrue);
            Assert.Equal("Hello", parser.ShouldBeHello);
            Assert.Equal(5000, parser.ShouldBeFiveThousand);
            Assert.Equal("Bye", parser.ShouldBeBye);
        }

        [Fact]
        public void MissingParametersTest()
        {
            string[] args = new[]
            {
                "--bool",
            };

            BuiltInTypesParser parser = new();
            Assert.Throws<MissingParametersException>(() => parser.Parse(args));
        }
    }

    public class BuiltInTypesParser : ArgumentsParser
    {
        [PropertyOption("bool")]
        public bool ShouldBeTrue { get; set; }

        [PropertyOption("string")]
        public string ShouldBeHello { get; set; }

        [PropertyOption("int")]
        public int ShouldBeFiveThousand { get; set; }

        [PropertyOption("dynamic")]
        public dynamic ShouldBeBye { get; set; }
    }
}
