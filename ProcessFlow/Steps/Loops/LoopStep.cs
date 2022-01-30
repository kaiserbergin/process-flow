using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class LoopStep<TState> : AbstractLoopStep<TState> where TState : class
    {
        private readonly ProcessAsyncFunc _processFunc;

        internal LoopStep(
            ProcessAsyncFunc processFunc,
            string? name = null, 
            StepSettings? stepSettings = null, 
            IClock? clock = null)
            : base(name, stepSettings, clock)
        {
            _processFunc = processFunc;
        }

        public delegate Task ProcessAsyncFunc(
            TState? state,
            Action? terminate,
            Action? @break,
            Action? @continue,
            int currentIteration = 0,
            CancellationToken cancellationToken = default);

        public static IStep<TState> Create(
            ProcessAsyncFunc processFunc,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) =>
            new LoopStep<TState>(processFunc, name, stepSettings, clock);


        protected override async Task ProcessAsync(TState? state, CancellationToken cancellationToken) =>
            await _processFunc(state, Terminate, Break, Continue, CurrentIteration, cancellationToken);
    }
}