using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps.Base
{
    public sealed class Step<TState> : AbstractStep<TState> where TState : class
    {
        private readonly Func<TState?, Action, CancellationToken, Task>? _processActionAsync;
        private readonly Action<TState?, Action>? _processActionSync;

        internal Step(Func<TState?, Action, CancellationToken, Task> processActionAsync, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) 
            : base(name, stepSettings, clock)
        {
            _processActionAsync = processActionAsync;
        }
        
        internal Step(Action<TState?, Action> processActionSync, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) 
            : base(name, stepSettings, clock)
        {
            _processActionSync = processActionSync;
        }
        
        public static IStep<TState> Create(Func<TState?, Task> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate, cancellationtoken) => processFunc(state), name, stepSettings, clock);
        
        public static IStep<TState> Create(Func<TState?, Action, Task> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate, cancellationtoken) => processFunc(state, terminate), name, stepSettings, clock);

        public static IStep<TState> Create(Func<TState?, Action, CancellationToken, Task> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>(processFunc, name, stepSettings, clock);
        
        public static IStep<TState> Create(Action<TState?> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate) => processFunc(state), name, stepSettings, clock);

        public static IStep<TState> Create(Action<TState?, Action> processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>(processFunc, name, stepSettings, clock);

        protected override async Task ProcessAsync(TState? state, CancellationToken cancellationToken)
        {
            if (_processActionAsync != null)
                await _processActionAsync(state, Terminate, cancellationToken);
            if (_processActionSync != null)
                _processActionSync(state, Terminate);
        }
    }
}