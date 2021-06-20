using System;
using Xunit;
using CliOptions;
using CliOptions.Exceptions;

namespace CliOptions.Tests
{
    public class PropertyOptionTests
    {
        private class BuiltInTypesParser : ArgumentsParser
        {
            [PropertyOption("bool")]
            public bool ShouldBeTrue { get; set; }

            [PropertyOption("string")]
            public string ShouldBeHello { get; set; }

            [PropertyOption("int")]
            public int ShouldBeFiveThousand { get; set; }
        }

        [Fact]
        public void BooleanArgumentTest()
        {
            string[] args = new[]
            {
                "--bool", "truE",
            };

            BuiltInTypesParser parser = new();
            parser.Parse(args);
            Assert.True(parser.ShouldBeTrue);
        }

        [Fact]
        public void StringArgumentTest()
        {
            string[] args = new[]
            {
                "--string", "Hello",
            };

            BuiltInTypesParser parser = new();
            parser.Parse(args);
            Assert.Equal("Hello", parser.ShouldBeHello);
        }

        [Fact]
        public void IntArgumentTest()
        {
            string[] args = new[]
            {
                "--int", "5000",
            };

            BuiltInTypesParser parser = new();
            parser.Parse(args);
            Assert.Equal(5000, parser.ShouldBeFiveThousand);
        }

        [Theory]
        [InlineData("--string")]
        [InlineData("--int")]
        [InlineData("--bool")]
        public void MissingParametersTest(params string[] args)
        {
            BuiltInTypesParser parser = new();
            Assert.Throws<MissingParametersException>(() => parser.Parse(args));
        }

        [Theory]
        [InlineData("--a")]
        [InlineData("-B")]
        public void UnknownOptionTest(params string[] args)
        {
            BuiltInTypesParser parser = new();
            Assert.Throws<UnknownOptionException>(() => parser.Parse(args));
        }
    }
}
