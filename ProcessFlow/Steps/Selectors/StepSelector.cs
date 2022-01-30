using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public sealed class StepSelector<TState> : AbstractStepSelector<TState> where TState: class
    {
        private readonly SelectAsyncDelegate? _selectAsync;
        private readonly SelectSyncDelegate? _selectSync;
        
        internal StepSelector(
            SelectAsyncDelegate selectAsync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null
            ) : base(name, stepSettings, options, clock)
        {
            _selectAsync = selectAsync;
        }
        
        internal StepSelector(
            SelectSyncDelegate selectSync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null
        ) : base(name, stepSettings, options, clock)
        {
            _selectSync = selectSync;
        }

        public delegate Task<List<IStep<TState>>> SelectAsyncDelegate(
            WorkflowState<TState>? state,
            Dictionary<string, IStep<TState>> options,
            Action terminate,
            CancellationToken cancellationToken = default);

        public static IStepSelector<TState> Create(
            SelectAsyncDelegate selectAsyncFunc,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) => new StepSelector<TState>(selectAsyncFunc, options, name, stepSettings, clock);
        
        public delegate List<IStep<TState>> SelectSyncDelegate(
            WorkflowState<TState>? state,
            Dictionary<string, IStep<TState>> options,
            Action terminate);
        
        public static IStepSelector<TState> Create(
            SelectSyncDelegate selectSync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) => new StepSelector<TState>(selectSync, options, name, stepSettings, clock);


        protected override async Task<List<IStep<TState>>> SelectAsync(WorkflowState<TState> workflowState, Dictionary<string, IStep<TState>> options, CancellationToken cancellationToken = default)
        {
            if (_selectAsync != null)
                return await _selectAsync(workflowState, options, Terminate, cancellationToken);
            return _selectSync != null ? _selectSync(workflowState, options, Terminate) : new List<IStep<TState>>();
        }
    }
}