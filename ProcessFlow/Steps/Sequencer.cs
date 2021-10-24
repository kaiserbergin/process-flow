using ProcessFlow.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public sealed class Sequencer<T> : Step<T> where T : class
    {
        private List<Step<T>> _sequence;

        public Sequencer(string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _sequence = new List<Step<T>>();
        }

        public Sequencer<T> AddStep(Step<T> processor)
        {
            _sequence.Add(processor);
            return this;
        }

        public List<Step<T>> GetSequence()
        {
            return _sequence;
        }

        public Sequencer<T> SetSequence(List<Step<T>> sequence)
        {
            _sequence = sequence;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            foreach (var process in _sequence)
            {
                workflowState = await process.Execute(workflowState, cancellationToken);
            }

            return workflowState;
        }

        protected override Task<T?> Process(T? state, CancellationToken cancellationToken) => Task.FromResult(state);
    }
}
