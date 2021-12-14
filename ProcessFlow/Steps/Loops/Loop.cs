using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public abstract class Loop<T> : AbstractStep<T> where T : class
    {
        protected int _currentIteration;
        protected List<AbstractStep<T>> _steps;

        protected Loop(string? name = null, StepSettings? stepSettings = null, List<AbstractStep<T>>? steps = null) : base(name, stepSettings)
        {
            _steps = steps ?? new List<AbstractStep<T>>();
        }

        public List<AbstractStep<T>> Steps => _steps;
        public int CurrentIteration => _currentIteration;

        public void SetSteps(List<AbstractStep<T>> steps) => _steps = steps;
        public void AddStep(AbstractStep<T> step) => _steps.Add(step);
        public void ClearSteps() => _steps = new List<AbstractStep<T>>();
        
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