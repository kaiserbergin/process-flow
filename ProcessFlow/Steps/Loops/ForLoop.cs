using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class ForLoop<TState> : AbstractLoop<TState> where TState : class
    {
        private int _iterationCount;
        private readonly Func<TState?, int>? _setIterationCount;
        private readonly Func<TState?, CancellationToken, Task<int>>? _setIterationCountAsync;

        public int IterationCount => _iterationCount;

        public ForLoop(
            int iterations,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) : base(name, stepSettings, steps, clock)
        {
            _iterationCount = iterations;
        }

        public ForLoop(
            Func<TState?, int> setIterationCount,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) : base(name, stepSettings, steps, clock)
        {
            _setIterationCount = setIterationCount;
        }

        public ForLoop(
            Func<TState?, CancellationToken, Task<int>> setIterationCountAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) : base(name, stepSettings, steps, clock)
        {
            _setIterationCountAsync = setIterationCountAsync;
        }

        public static ForLoop<TState> Create(
            int iterations,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) => new ForLoop<TState>(iterations, name, stepSettings, steps, clock);

        public static ForLoop<TState> Create(
            Func<TState?, int> setIterationCount,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) => new ForLoop<TState>(setIterationCount, name, stepSettings, steps, clock);

        public static ForLoop<TState> Create(
            Func<TState?, CancellationToken, Task<int>> setIterationCountAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<TState>>? steps = null,
            IClock? clock = null) => new ForLoop<TState>(setIterationCountAsync, name, stepSettings, steps, clock);

        protected override async Task ProcessAsync(TState? state, CancellationToken cancellationToken)
        {
            if (_setIterationCount != null)
                _iterationCount = _setIterationCount(state);
            else if (_setIterationCountAsync != null)
                _iterationCount = await _setIterationCountAsync(state, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            for (var i = 0; i < _iterationCount; i++)
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
    }
}