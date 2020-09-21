using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps.Loops
{
    public abstract class Loop<T> : Step<T> where T : class
    {
        protected int _currentIteration = 0;
        protected List<Step<T>> _steps;

        protected Loop(string name = null, StepSettings stepSettings = null, List<Step<T>> steps = null) : base(name, stepSettings)
        {
            _steps = steps ?? new List<Step<T>>();
        }

        public List<Step<T>> Steps => _steps;
        public int CurrentIteration => _currentIteration;
        
        public void SetSteps(List<Step<T>> steps) => _steps = steps ?? new List<Step<T>>();
        public void AddStep(Step<T> step) => _steps.Add(step);
        public void ClearSteps() => _steps = new List<Step<T>>();
        
        protected async Task Iterate(WorkflowState<T> workflowState)
        {
            foreach (var step in _steps)
            {
                if (step is LoopStep<T> loopStep)
                    loopStep.SetIteration(_currentIteration);

                await step.Execute(workflowState);
            }
        }
    }
}