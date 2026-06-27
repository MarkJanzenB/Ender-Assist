using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Exceptions;

namespace MarlinPrintMiddleware.Queue;

public sealed class PrintStateMachine
{
    public PrintState State { get; private set; } = PrintState.Idle;

    public void TransitionTo(PrintState next)
    {
        if (!CanTransition(State, next))
        {
            throw new PrintQueueException($"Invalid print state transition: {State} -> {next}");
        }

        State = next;
    }

    public void Reset() => State = PrintState.Idle;

    public static bool CanTransition(PrintState current, PrintState next) => (current, next) switch
    {
        (PrintState.Idle, PrintState.Preparing) => true,
        (PrintState.Preparing, PrintState.Printing) => true,
        (PrintState.Preparing, PrintState.Failed) => true,
        (PrintState.Preparing, PrintState.Cancelled) => true,
        (PrintState.Printing, PrintState.Paused) => true,
        (PrintState.Printing, PrintState.Completed) => true,
        (PrintState.Printing, PrintState.Failed) => true,
        (PrintState.Printing, PrintState.Cancelled) => true,
        (PrintState.Paused, PrintState.Printing) => true,
        (PrintState.Paused, PrintState.Cancelled) => true,
        (PrintState.Paused, PrintState.Failed) => true,
        (_, PrintState.Idle) => true,
        _ => false
    };
}
