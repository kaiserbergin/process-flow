using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Sequencers
{
    public sealed class Sequencer<TState> : AbstractStep<TState>, ISequencer<TState> where TState : class
    {
        private List<IStep<TState>> _sequence;

        public Sequencer(List<IStep<TState>>? steps = null, string? name = null, StepSettings? stepSettings = null, IClock? clock = null) : base(name, stepSettings, clock)
        {
            _sequence = steps ?? new List<IStep<TState>>();
        }

        public static ISequencer<TState> Create(
            List<IStep<TState>>? steps = null,
            string? name = null,
            StepSettings? stepSettings = null,
            IClock? clock = null) => new Sequencer<TState>(steps, name, stepSettings, clock);

        public ISequencer<TState> AddStep(IStep<TState> processor)
        {
            _sequence.Add(processor);
            return this;
        }

        public List<IStep<TState>> GetSequence()
        {
            return _sequence;
        }

        public ISequencer<TState> SetSequence(List<IStep<TState>> sequence)
        {
            _sequence = sequence;
            return this;
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            foreach (var process in _sequence)
            {
                workflowState = await process.ExecuteAsync(workflowState, cancellationToken);
            }
        }

        protected override Task ProcessAsync(TState? state, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
