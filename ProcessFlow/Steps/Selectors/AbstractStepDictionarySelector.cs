using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public abstract class AbstractStepDictionarySelector<TState> : AbstractStep<TState>, IStepDictionarySelector<TState> where TState : class
    {
        private Dictionary<string, IStep<TState>> _options;

        public AbstractStepDictionarySelector(string? name = null, StepSettings? stepSettings = null, Dictionary<string, IStep<TState>>? options = null) : base(name, stepSettings)
        {
            _options = options ?? new Dictionary<string, IStep<TState>>();
        }

        public Dictionary<string, IStep<TState>> Options() => _options;

        public AbstractStepDictionarySelector<TState> SetOptions(Dictionary<string, IStep<TState>> options)
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

        protected abstract Task<List<IStep<TState>>> SelectAsync(Dictionary<string, IStep<TState>> options, WorkflowState<TState> workflowState, CancellationToken? cancellationToken = default);
    }
}