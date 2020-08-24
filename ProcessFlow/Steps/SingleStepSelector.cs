using ProcessFlow.Data;
using ProcessFlow.Flow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public abstract class SingleStepSelector<T> : Step<T> where T : class
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

        /// <summary>
        /// Sets the options for a <see cref="SingleStepSelector{T}"/> object and then returns an
        /// object that can be used to set the next step for each step in <paramref
        /// name="options"/> to the same next step. If any of the values in <paramref
        /// name="options"/> need to have a different next step, do not use this method. This
        /// method is only designed to be used when there is a temporary, one stage split and then
        /// the workflow converges immediately afterwards.
        /// </summary>
        /// <param name="stepSelector">The step that the options will be applied to.</param>
        /// <param name="options">The options to be applied to <paramref name="source"/>.</param>
        /// <returns>An object that can be used to set the next step for all steps in <paramref name="options"/>.</returns>
        public SetOptionsConvergeResult<T> SetOptionsConverge(List<Step<T>> options)
        {
            SetOptions(options);

            return new SetOptionsConvergeResult<T>(options);
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
