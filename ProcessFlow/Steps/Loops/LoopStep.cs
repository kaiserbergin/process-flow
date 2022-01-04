using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class LoopStep<TState> : AbstractLoopStep<TState> where TState : class
    {
        private readonly Func<TState?, TerminateDelegate, BreakDelegate, ContinueDelegate, int, CancellationToken, Task> _processFunc;

        internal LoopStep(
            Func<TState?, TerminateDelegate, BreakDelegate, ContinueDelegate, int, CancellationToken, Task> processFunc,
            string? name = null, 
            StepSettings? stepSettings = null, 
            IClock? clock = null)
            : base(name, stepSettings, clock)
        {
            _processFunc = processFunc;
        }

        public static IStep<TState> Create(
            Func<TState?, TerminateDelegate, BreakDelegate, ContinueDelegate, int, CancellationToken, Task> processFunc,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) =>
            new LoopStep<TState>(processFunc, name, stepSettings, clock);

        public static IStep<TState> Create(
            Func<TState?, TerminateDelegate, BreakDelegate, ContinueDelegate, int, Task> processFunc,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) =>
            new LoopStep<TState>(
                (state, terminate, @break, @continue, currentIteration, cancellationToken) => processFunc(state, terminate, @break, @continue, currentIteration),
                name,
                stepSettings,
                clock);

        protected override async Task ProcessAsync(TState? state, CancellationToken cancellationToken) =>
            await _processFunc(state, Terminate, Break, Continue, CurrentIteration, cancellationToken);
    }
}