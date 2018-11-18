﻿using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void NoSpaces()
        {
            AssertExtensions.AnalyzeWithAnyMessage("X=XX=X");
        }

        [Fact]
        public void Multiline()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(
                "X = X\n" +
                "X2 = X2");
        }

        [Fact]
        public void MultipleWhiteSpaces()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(" X  =    X    ");
        }

        [Fact]
        public void Tabs()
        {
            AssertExtensions.AnalyzeWithEmptyMessages("\t X = X");
        }

        [Fact]
        public void GoToMacro()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(
                "MACRO GOTO L\n" +
                "Z = Z + 1\n" +
                "IF Z != 0 GOTO L\n" +
                "END\n" +
                "GOTO E");
        }

        [Fact]
        public void ZeroMacro()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(
                "MACRO ZERO K V\n" +
                "[K] V = V - 1\n" +
                "IF V != 0 GOTO K\n" +
                "END\n" +
                "X = X + 1\n" +
                "ZERO A X");
        }

        [Fact]
        public void ParamIncludedInNameMacro()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(
                "MACRO AA A\n" +
                "A = A\n" +
                "END\n" +
                "AA X");
        }

        [Fact]
        public void MultipleMacros()
        {
            AssertExtensions.AnalyzeWithEmptyMessages(
                "MACRO FIRST V\n" +
                "V = V\n" +
                "END\n" +
                "MACRO SECOND V\n" +
                "V = V\n" +
                "END\n" +
                "FIRST X\n" +
                "SECOND X");
        }

        [Fact]
        public void RandomInput()
        {
            AssertExtensions.AnalyzeWithAnyMessage("Lorem ipsum");
        }
    }
}
