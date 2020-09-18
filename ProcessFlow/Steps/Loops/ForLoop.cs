using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;

namespace ProcessFlow.Steps.Loops
{
    public sealed class ForLoop<T> : LoopController<T> where T : class
    {
        private int _iterationCount = 0;
        private readonly Func<T, int> _determineIterationCount;
        private readonly Func<T, Task<int>> _determineIterationCountAsync;

        public ForLoop(
            int iterations,
            string name = null,
            StepSettings stepSettings = null,
            List<Step<T>> steps = null) : base(name, stepSettings, steps)
        {
            _iterationCount = iterations;
        }

        public ForLoop(
            Func<T, int> determineIterationCount,
            string name = null,
            StepSettings stepSettings = null,
            List<Step<T>> steps = null) : base(name, stepSettings, steps)
        {
            _determineIterationCount = determineIterationCount;
        }

        public ForLoop(
            Func<T, Task<int>> determineIterationCountAsync,
            string name = null,
            StepSettings stepSettings = null,
            List<Step<T>> steps = null) : base(name, stepSettings, steps)
        {
            _determineIterationCountAsync = determineIterationCountAsync;
        }

        protected override async Task<T> Process(T state)
        {
            if (_determineIterationCount != null)
                _iterationCount = _determineIterationCount(state);
            else if (_determineIterationCountAsync != null)
                _iterationCount = await _determineIterationCountAsync(state).ConfigureAwait(false);

            return state;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState)
        {
            for (var i = 0; i < _iterationCount; i++)
            {
                try
                {
                    foreach (var step in _steps)
                    {
                        if (step is LoopStep<T> controlStep)
                            controlStep.SetIteration(i);
                        
                        await step.Execute(workflowState);
                    }
                }
                catch (ContinueException)
                {
                    _currentIteration++;
                    continue;
                }
                catch (BreakException)
                {
                    break;
                }

                _currentIteration++;
            }
            return workflowState;
        }
    }
}