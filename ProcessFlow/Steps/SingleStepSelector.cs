using ProcessFlow.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public abstract class SingleStepSelector<T> : Step<T>
    {
        private List<Step<T>> _options;
        public SingleStepSelector(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
            _options = new List<Step<T>>();
        }

        public List<Step<T>> Options()
        {
            return _options;
        }

        public SingleStepSelector<T> SetOptions(List<Step<T>> options)
        {
            _options = options;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState)
        {
            var selectedProcessor = await Select(_options, workflowState);
            this.SetNext(selectedProcessor);
            return workflowState;
        }

        protected override Task<T> Process(T state)
        {
            return Task.FromResult(state);
        }

        protected abstract Task<Step<T>> Select(List<Step<T>> options, WorkflowState<T> workflowState);

    }
}
