using GoTo;
using GoTo.Interpreter;
using GoTo.Parser.AbstractSyntaxTree;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class InterpreterTests : AbstractRunTests
    {
        public InterpreterTests() : base(true)
        { }

        [Fact]
        public void InfiniteLoop()
        {
            Framework.TryAnalyze(
                "[A] X = X + 1 " +
                "IF X != 0 GOTO A", 
                out string _, 
                out ProgramNode program, 
                out IEnumerable<Message> _);

            var isExpectedExceptionCatched = false;

            try
            {
                Framework.RunInterpreted(program, out int _);
            }
            catch (MaxStepsExceededException)
            {
                isExpectedExceptionCatched = true;
            }

            Assert.True(isExpectedExceptionCatched);
        }

        [Fact]
        public void OneStepDebugger()
        {
            var isTestFinished = false;

            Framework.TryAnalyze("Y = Y + 1", out string _, out ProgramNode program, out IEnumerable<Message> _);
            Framework.RunInterpreted(
                program,
                out int _,
                stepDebugAndContinue: state =>
                {
                    Assert.False(isTestFinished);
                    Assert.Equal(1, state.Y);

                    isTestFinished = true;

                    return false;
                });

            Assert.True(isTestFinished, "Test should have succeeded after executing one step.");
        }

        [Fact]
        public void TwoStepsDebugger()
        {
            var stepsTaken = 0;

            Framework.TryAnalyze(
                "Y = Y + 1 " +
                "Y = Y + 1", 
                out string _, 
                out ProgramNode program, 
                out IEnumerable<Message> _);
            Framework.RunInterpreted(
                program,
                out int _,
                stepDebugAndContinue: state =>
                {
                    stepsTaken++;

                    return true;
                });

            Assert.Equal(2, stepsTaken);
        }
    }
}
