using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps.Loops
{
    public sealed class LoopStep<TState> : AbstractLoopStep<TState> where TState : class
    {
        private readonly Func<TState?, Action, Action, Action, int, CancellationToken, Task<TState?>> _processFunc;

        internal LoopStep(Func<TState?, Action, Action, Action, int, CancellationToken, Task<TState?>> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null)
            : base(name, stepSettings, clock)
        {
            _processFunc = processFunc;
        }

        public static IStep<TState> Create(
            Func<TState?, Action, Action, Action, int, CancellationToken, Task<TState?>> processFunc,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) =>
            new LoopStep<TState>(processFunc, name, stepSettings, clock);

        public static IStep<TState> Create(
            Func<TState?, Action, Action, Action, int, Task<TState?>> processFunc,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) =>
            new LoopStep<TState>(
                (state, terminate, @break, @continue, currentIteration, cancellationToken) => processFunc(state, terminate, @break, @continue, currentIteration),
                name,
                stepSettings,
                clock);

        protected override async Task<TState?> ProcessAsync(TState? state, CancellationToken cancellationToken) =>
            await _processFunc(state, Terminate, Break, Continue, CurrentIteration, cancellationToken);
    }
}