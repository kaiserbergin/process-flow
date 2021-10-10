using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;

namespace ProcessFlow.Steps.Loops
{
    public sealed class WhileLoop<T> : Loop<T> where T : class
    {
        private readonly Func<T?, bool>? _shouldContinue;
        private readonly Func<T?, Task<bool>>? _shouldContinueAsync;

        public WhileLoop(string? name = null, StepSettings? stepSettings = null, List<Step<T>>? steps = null) : base(name, stepSettings, steps) { }
        public WhileLoop(
            Func<T?, bool> shouldContinue,
            string? name = null,
            StepSettings? stepSettings = null,
            List<Step<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinue = shouldContinue;
        }

        public WhileLoop(
            Func<T?, Task<bool>> shouldContinueAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<Step<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinueAsync = shouldContinueAsync;
        }

        protected override Task<T?> Process(T? state) => Task.FromResult(state);

        protected override async Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState)
        {
            while (await ShouldContinue(workflowState.State))
            {
                try
                {
                    await Iterate(workflowState);
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

        private async Task<bool> ShouldContinue(T? state)
        {
            if (_shouldContinue != null)
                return _shouldContinue(state);
            if (_shouldContinueAsync != null)
                return await _shouldContinueAsync(state);
            return true;
        }
    }
}