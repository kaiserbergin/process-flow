using ProcessFlow.Data;
using ProcessFlow.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessFlow.Flow
{
    public class Selector<T> : Step<T>, ISelector<T>
    {
        private List<Step<T>> _options;

        private ISingleStepSelector<T> _stepSelector;

        public Selector() : base()
        {
            _options = new List<Step<T>>();
        }

        public Selector(string name = null, string id = null, StepSettings stepSettings = null, ISingleStepSelector<T> stepSelector = null) : base(name, id, stepSettings)
        {
            _options = new List<Step<T>>();
            _stepSelector = stepSelector;
        }

        public Selector<T> SetStepSelector(ISingleStepSelector<T> stepSelector)
        {
            _stepSelector = stepSelector;
            return this;
        }

        public Selector<T> SetOptions(List<Step<T>> options)
        {
            _options = options;
            return this;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState)
        {
            if (_stepSelector != null)
            {
                var selectedProcessor = await _stepSelector.Select(_options, workflowState);
                this.SetNext(selectedProcessor);
            }

            return workflowState;
        }

        public List<Step<T>> Options()
        {
            return _options;
        }
    }
}
