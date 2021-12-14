using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public abstract class AbstractStepListSelector<TState> : AbstractStep<TState>, IStepListSelector<TState> where TState : class
    {
        private List<IStep<TState>> _options;
        public AbstractStepListSelector(List<IStep<TState>>? options = null, string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _options = options ?? new List<IStep<TState>>();
        }

        public List<IStep<TState>> Options()
        {
            return _options;
        }

        public AbstractStepListSelector<TState> SetOptions(List<IStep<TState>> options)
        {
            _options = options;
            return this;
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            var selectedProcessors = await SelectAsync(_options, workflowState, cancellationToken);
            
            foreach (var process in selectedProcessors)
            {
                workflowState = await process.ExecuteAsync(workflowState, cancellationToken);
            }
        }

        protected override Task ProcessAsync(TState? state, CancellationToken cancellationToken) => Task.CompletedTask;

        protected abstract Task<List<IStep<TState>>> SelectAsync(List<IStep<TState>> options, WorkflowState<TState> workflowState, CancellationToken? cancellationToken = default);
    }
}
