using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;

namespace ProcessFlow.Steps.Loops
{
    public sealed class ForLoop<T> : Loop<T> where T : class
    {
        private int _iterationCount;
        private readonly Func<T?, int>? _setIterationCount;
        private readonly Func<T?, CancellationToken, Task<int>>? _setIterationCountAsync;

        public ForLoop(
            int iterations,
            string? name = null,
            StepSettings? stepSettings = null,
            List<Step<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _iterationCount = iterations;
        }
        
        public ForLoop(
            Func<T?, int> setIterationCount,
            string? name = null,
            StepSettings? stepSettings = null,
            List<Step<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _setIterationCount = setIterationCount;
        }

        public ForLoop(
            Func<T?, CancellationToken, Task<int>> setIterationCountAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<Step<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _setIterationCountAsync = setIterationCountAsync;
        }

        protected override async Task<T?> Process(T? state, CancellationToken cancellationToken = default)
        {
            if (_setIterationCount != null)
                _iterationCount = _setIterationCount(state);
            else if (_setIterationCountAsync != null)
                _iterationCount = await _setIterationCountAsync(state, cancellationToken).ConfigureAwait(false);

            return state;
        }

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            for (var i = 0; i < _iterationCount; i++)
            {
                try
                {
                    await Iterate(workflowState, cancellationToken);
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