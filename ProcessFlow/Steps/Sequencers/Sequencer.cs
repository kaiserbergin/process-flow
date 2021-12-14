using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Sequencers
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

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            foreach (var process in _sequence)
            {
                workflowState = await process.ExecuteAsync(workflowState, cancellationToken);
            }
        }

        protected override Task ProcessAsync(T? state, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
