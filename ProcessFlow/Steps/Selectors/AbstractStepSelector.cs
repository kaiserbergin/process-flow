using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public abstract class AbstractStepSelector<TState> : AbstractStep<TState>, IStepSelector<TState> where TState : class
    {
        private Dictionary<string, IStep<TState>> _options;

        public AbstractStepSelector(
            string? name = null, 
            StepSettings? stepSettings = null, 
            Dictionary<string, IStep<TState>>? options = null, 
            IClock? clock = null) 
            : base(name, stepSettings, clock)
        {
            _options = options ?? new Dictionary<string, IStep<TState>>();
        }

        public Dictionary<string, IStep<TState>> Options() => _options;

        public IStepSelector<TState> SetOptions(Dictionary<string, IStep<TState>> options)
        {
            _options = options;
            return this;
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            var selectedProcessors = await SelectAsync(workflowState, _options, cancellationToken);
            
            foreach (var process in selectedProcessors)
            {
                workflowState = await process.ExecuteAsync(workflowState, cancellationToken);
            }
        }

        protected override Task ProcessAsync(TState? state, CancellationToken cancellationToken) => Task.CompletedTask;

        protected abstract Task<List<IStep<TState>>> SelectAsync(WorkflowState<TState> workflowState, Dictionary<string, IStep<TState>> options, CancellationToken cancellationToken = default);
    }
}