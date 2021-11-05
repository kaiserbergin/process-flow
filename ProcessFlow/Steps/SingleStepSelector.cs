using ProcessFlow.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public abstract class SingleStepSelector<T> : AbstractStep<T> where T : class
    {
        private List<AbstractStep<T>> _options;
        public SingleStepSelector(string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _options = new List<AbstractStep<T>>();
        }

        public List<AbstractStep<T>> Options()
        {
            return _options;
        }

        public SingleStepSelector<T> SetOptions(List<AbstractStep<T>> options)
        {
            _options = options;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            var selectedProcessor = await SelectAsync(_options, workflowState, cancellationToken);
            await selectedProcessor.ExecuteAsync(workflowState, cancellationToken);
            return workflowState;
        }

        protected override Task<T?> ProcessAsync(T? state, CancellationToken cancellationToken)
        {
            return Task.FromResult(state);
        }

        protected abstract Task<AbstractStep<T>> SelectAsync(List<AbstractStep<T>> options, WorkflowState<T> workflowState, CancellationToken cancellationToken);
    }
}
