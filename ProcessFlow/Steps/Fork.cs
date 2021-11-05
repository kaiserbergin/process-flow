using ProcessFlow.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public sealed class Fork<T> : AbstractStep<T> where T : class
    {
        private List<AbstractStep<T>> _steps;

        public Fork(string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _steps = new List<AbstractStep<T>>();
        }

        public Fork(List<AbstractStep<T>> steps, string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _steps = steps;
        }

        public Fork(string? name = null, StepSettings? stepSettings = null, params AbstractStep<T>[] steps) : base(name, stepSettings)
        {
            _steps = steps.ToList();
        }

        public Fork<T> AddStep(AbstractStep<T> processor)
        {
            _steps.Add(processor);
            return this;
        }

        public List<AbstractStep<T>> GetSteps()
        {
            return _steps;
        }

        public Fork<T> SetSteps(List<AbstractStep<T>> sequence)
        {
            _steps = sequence;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>();

            foreach (var process in _steps)
            {
                taskList.Add(process.ExecuteAsync(workflowState, cancellationToken));
            }

            await Task.WhenAll(taskList);

            return workflowState;
        }

        protected override Task<T?> ProcessAsync(T? state, CancellationToken cancellationToken) => Task.FromResult(state);
    }
}
