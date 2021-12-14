using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Forks
{
    public sealed class Fork<T> : AbstractStep<T>, IFork<T> where T : class
    {
        private List<IStep<T>> _steps;

        public Fork(string? name = null, StepSettings? stepSettings = null, List<IStep<T>>? steps = null) : base(name, stepSettings)
        {
            _steps = steps ?? new List<IStep<T>>();
        }

        public Fork(string? name = null, StepSettings? stepSettings = null, params IStep<T>[] steps) : base(name, stepSettings)
        {
            _steps = steps.ToList();
        }

        public static IFork<T> Create(string? name = null, StepSettings? stepSettings = null, List<IStep<T>>? steps = null) => new Fork<T>(name, stepSettings, steps);

        public static IFork<T> Create(string? name = null, StepSettings? stepSettings = null, params IStep<T>[] steps) => new Fork<T>(name, stepSettings, steps);

        public Fork<T> AddStep(IStep<T> processor)
        {
            _steps.Add(processor);
            return this;
        }

        public List<IStep<T>> GetSteps()
        {
            return _steps;
        }

        public Fork<T> SetSteps(List<IStep<T>> sequence)
        {
            _steps = sequence;
            return this;
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            var taskList = new List<Task>();

            foreach (var process in _steps)
            {
                taskList.Add(process.ExecuteAsync(workflowState, cancellationToken));
            }

            await Task.WhenAll(taskList);
        }

        protected override Task ProcessAsync(T? state, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
