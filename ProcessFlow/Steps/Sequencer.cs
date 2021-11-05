using ProcessFlow.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public sealed class Sequencer<T> : AbstractStep<T> where T : class
    {
        private List<AbstractStep<T>> _sequence;

        public Sequencer(string? name = null, StepSettings? stepSettings = null) : base(name, stepSettings)
        {
            _sequence = new List<AbstractStep<T>>();
        }

        public Sequencer<T> AddStep(AbstractStep<T> processor)
        {
            _sequence.Add(processor);
            return this;
        }

        public List<AbstractStep<T>> GetSequence()
        {
            return _sequence;
        }

        public Sequencer<T> SetSequence(List<AbstractStep<T>> sequence)
        {
            _sequence = sequence;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            foreach (var process in _sequence)
            {
                workflowState = await process.ExecuteAsync(workflowState, cancellationToken);
            }

            return workflowState;
        }

        protected override Task<T?> ProcessAsync(T? state, CancellationToken cancellationToken) => Task.FromResult(state);
    }
}
