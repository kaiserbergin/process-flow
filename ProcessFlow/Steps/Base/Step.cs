using System;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps.Base
{
    public sealed class Step<TState> : AbstractStep<TState> where TState : class
    {
        private readonly ProcessActionAsync? _processActionAsync;
        private readonly ProcessActionSync? _processActionSync;

        internal Step(ProcessActionAsync processActionAsync, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) 
            : base(name, stepSettings, clock)
        {
            _processActionAsync = processActionAsync;
        }
        
        internal Step(ProcessActionSync processActionSync, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) 
            : base(name, stepSettings, clock)
        {
            _processActionSync = processActionSync;
        }

        public delegate Task ProcessActionAsync(TState? state, Action terminate, CancellationToken cancellationToken = default);
        public delegate Task ProcessActionAsyncStub1(TState? state);
        public delegate Task ProcessActionAsyncStub2(TState? state, Action terminate);
        
        public static IStep<TState> Create(ProcessActionAsyncStub1 processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate, cancellationtoken) => processFunc(state), name, stepSettings, clock);
        
        public static IStep<TState> Create(ProcessActionAsyncStub2 processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate, cancellationtoken) => processFunc(state, terminate), name, stepSettings, clock);

        public static IStep<TState> Create(ProcessActionAsync processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>(processFunc, name, stepSettings, clock);

        public delegate void ProcessActionSync(TState? state, Action terminate);
        public delegate void ProcessActionSyncStub1(TState? state);
        
        public static IStep<TState> Create(ProcessActionSyncStub1 processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>((state, terminate) => processFunc(state), name, stepSettings, clock);

        public static IStep<TState> Create(ProcessActionSync processFunc, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) =>
            new Step<TState>(processFunc, name, stepSettings, clock);

        protected override async Task ProcessAsync(TState? state, CancellationToken cancellationToken)
        {
            if (_processActionAsync != null)
                await _processActionAsync(state, Terminate, cancellationToken);
            
            _processActionSync?.Invoke(state, Terminate);
        }
    }
}