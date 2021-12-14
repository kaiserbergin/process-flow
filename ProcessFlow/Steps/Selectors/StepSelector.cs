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
        private readonly Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, CancellationToken, Task<List<IStep<TState>>>>? _selectAsync;
        private readonly Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, List<IStep<TState>>>? _selectSync;
        
        internal StepSelector(
            Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, CancellationToken, Task<List<IStep<TState>>>> selectAsync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null
            ) : base(name, stepSettings, options, clock)
        {
            _selectAsync = selectAsync;
        }
        
        internal StepSelector(
            Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, List<IStep<TState>>> selectSync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null
        ) : base(name, stepSettings, options, clock)
        {
            _selectSync = selectSync;
        }


        public static IStepSelector<TState> Create(
            Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, CancellationToken, Task<List<IStep<TState>>>> selectAsyncFunc,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) => new StepSelector<TState>(selectAsyncFunc, options, name, stepSettings, clock);
        
        public static IStepSelector<TState> Create(
            Func<WorkflowState<TState>?, Dictionary<string, IStep<TState>>, Action, List<IStep<TState>>> selectSync,
            Dictionary<string, IStep<TState>> options,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) => new StepSelector<TState>(selectSync, options, name, stepSettings, clock);


        protected override async Task<List<IStep<TState>>> SelectAsync(WorkflowState<TState> workflowState, Dictionary<string, IStep<TState>> options, CancellationToken cancellationToken = default)
        {
            if (_selectAsync != null)
                return await _selectAsync(workflowState, options, Terminate, cancellationToken);
            if (_selectSync != null)
                return _selectSync(workflowState, options, Terminate);

            return new List<IStep<TState>>();
        }
    }
}