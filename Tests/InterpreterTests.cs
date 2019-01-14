using GoTo;
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
                out ProgramNode program, 
                out IEnumerable<Message> _);
            Framework.TryRunInterpreted(program, out int _);
        }

        [Fact]
        public void OneStepDebugger()
        {
            var isTestFinished = false;

            Framework.TryAnalyze("Y = Y + 1", out ProgramNode program, out IEnumerable<Message> _);
            Framework.TryRunInterpreted(
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
                out ProgramNode program, 
                out IEnumerable<Message> _);
            Framework.TryRunInterpreted(
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
