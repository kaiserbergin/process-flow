using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class WhileLoop<TState> : AbstractLoop<TState> where TState : class
    {
        private readonly Func<TState?, bool>? _shouldContinue;
        private readonly Func<TState?, CancellationToken, Task<bool>>? _shouldContinueAsync;

        public WhileLoop(string? name = null, StepSettings? stepSettings = null, List<IStep<TState>>? steps = null) : base(name, stepSettings, steps) { }
        public WhileLoop(
            Func<TState?, bool> shouldContinue,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinue = shouldContinue;
        }

        public WhileLoop(
            Func<TState?, CancellationToken, Task<bool>> shouldContinueAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null) : base(name, stepSettings, steps)
        {
            _shouldContinueAsync = shouldContinueAsync;
        }

        public static WhileLoop<TState> Create(
            Func<TState?, bool> shouldContinue,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null) => new WhileLoop<TState>(shouldContinue, name, stepSettings, steps);

        public static WhileLoop<TState> Create(
            Func<TState?, CancellationToken, Task<bool>> shouldContinueAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null) => new WhileLoop<TState>(shouldContinueAsync, name, stepSettings, steps);

        protected override Task ProcessAsync(TState? state, CancellationToken cancellationToken) => Task.CompletedTask;

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
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

        private async Task<bool> ShouldContinueAsync(TState? state, CancellationToken cancellationToken)
        {
            if (_shouldContinue != null)
                return _shouldContinue(state);
            if (_shouldContinueAsync != null)
                return await _shouldContinueAsync(state, cancellationToken);
            return true;
        }
    }
}