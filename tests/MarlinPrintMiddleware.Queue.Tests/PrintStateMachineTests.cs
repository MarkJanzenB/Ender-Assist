using FluentAssertions;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Queue;

namespace MarlinPrintMiddleware.Queue.Tests;

public class PrintStateMachineTests
{
    [Fact]
    public void ValidTransitions_Succeed()
    {
        var machine = new PrintStateMachine();
        machine.TransitionTo(PrintState.Preparing);
        machine.TransitionTo(PrintState.Printing);
        machine.TransitionTo(PrintState.Paused);
        machine.TransitionTo(PrintState.Printing);
        machine.TransitionTo(PrintState.Completed);
        machine.State.Should().Be(PrintState.Completed);
    }

    [Fact]
    public void InvalidTransition_Throws()
    {
        var machine = new PrintStateMachine();
        machine.Invoking(m => m.TransitionTo(PrintState.Printing))
            .Should().Throw<MarlinPrintMiddleware.Core.Exceptions.PrintQueueException>();
    }
}
