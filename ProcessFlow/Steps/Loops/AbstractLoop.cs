using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public abstract class AbstractLoop<T> : AbstractStep<T> where T : class
    {
        protected int _currentIteration;
        protected List<IStep<T>> _steps;

        protected AbstractLoop(string? name = null, StepSettings? stepSettings = null, List<IStep<T>>? steps = null) : base(name, stepSettings)
        {
            _steps = steps ?? new List<IStep<T>>();
        }

        public List<IStep<T>> Steps => _steps;
        public int CurrentIteration => _currentIteration;

        public void SetSteps(List<IStep<T>> steps) => _steps = steps;
        public void AddStep(IStep<T> step) => _steps.Add(step);
        public void ClearSteps() => _steps = new List<IStep<T>>();
        
        protected async Task IterateAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            foreach (var step in _steps)
            {
                if (step is AbstractLoopStep<T> loopStep)
                    loopStep.SetIteration(_currentIteration);

                await step.ExecuteAsync(workflowState, cancellationToken);
            }
        }
    }
}