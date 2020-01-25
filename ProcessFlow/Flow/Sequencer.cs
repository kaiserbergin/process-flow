using ProcessFlow.Data;
using ProcessFlow.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessFlow.Flow
{
    public sealed class Sequencer<T> : Step<T>, ISequencer<T>
    {
        private List<Step<T>> _sequence;

        public Sequencer() : base()
        {
            _sequence = new List<Step<T>>();
        }

        public Sequencer(string name = null, string id = null, StepSettings stepSettings = null) : base(name, id, stepSettings)
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

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState)
        {
            foreach (var process in _sequence)
            {
                workflowState = await process.Process(workflowState);
            }

            return workflowState;
        }
    }
}
