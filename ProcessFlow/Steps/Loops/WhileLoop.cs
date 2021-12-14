using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class WhileLoop<T> : AbstractLoop<T> where T : class
    {
        private readonly Func<T?, bool>? _shouldContinue;
        private readonly Func<T?, CancellationToken, Task<bool>>? _shouldContinueAsync;

        public WhileLoop(string? name = null, StepSettings? stepSettings = null, List<IStep<T>>? steps = null) : base(name, stepSettings, steps) { }
        public WhileLoop(
            Func<T?, bool> shouldContinue,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinue = shouldContinue;
        }

        public WhileLoop(
            Func<T?, CancellationToken, Task<bool>> shouldContinueAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinueAsync = shouldContinueAsync;
        }

        public static WhileLoop<T> Create(
            Func<T?, bool> shouldContinue,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) => new WhileLoop<T>(shouldContinue, name, stepSettings);

        public static WhileLoop<T> Create(
            Func<T?, CancellationToken, Task<bool>> shouldContinueAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) => new WhileLoop<T>(shouldContinueAsync, name, stepSettings);

        protected override Task ProcessAsync(T? state, CancellationToken cancellationToken) => Task.CompletedTask;

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
        {
            while (await ShouldContinueAsync(workflowState.State, cancellationToken))
            {
                try
                {
                    await IterateAsync(workflowState, cancellationToken);
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
        }

        private async Task<bool> ShouldContinueAsync(T? state, CancellationToken cancellationToken)
        {
            if (_shouldContinue != null)
                return _shouldContinue(state);
            if (_shouldContinueAsync != null)
                return await _shouldContinueAsync(state, cancellationToken);
            return true;
        }
    }
}