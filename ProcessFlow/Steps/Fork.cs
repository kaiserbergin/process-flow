using ProcessFlow.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public sealed class Fork<T> : Step<T> where T : class
    {
        private List<Step<T>> _steps;

        public Fork(string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _steps = new List<Step<T>>();
        }

        public Fork(List<Step<T>> steps, string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _steps = steps;
        }

        public Fork(string? name = null, StepSettings? stepSettings = null, params Step<T>[] steps) : base(name, stepSettings)
        {
            _steps = steps.ToList();
        }

        public Fork<T> AddStep(Step<T> processor)
        {
            _steps.Add(processor);
            return this;
        }

        public List<Step<T>> GetSteps()
        {
            return _steps;
        }

        public Fork<T> SetSteps(List<Step<T>> sequence)
        {
            _steps = sequence;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>();

            foreach (var process in _steps)
            {
                taskList.Add(process.Execute(workflowState, cancellationToken));
            }

            await Task.WhenAll(taskList);

            return workflowState;
        }

        protected override Task<T?> Process(T? state, CancellationToken cancellationToken) => Task.FromResult(state);
    }
}
