using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public sealed class ForLoop<T> : AbstractLoop<T> where T : class
    {
        private int _iterationCount;
        private readonly Func<T?, int>? _setIterationCount;
        private readonly Func<T?, CancellationToken, Task<int>>? _setIterationCountAsync;

        public ForLoop(
            int iterations,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _iterationCount = iterations;
        }

        public ForLoop(
            Func<T?, int> setIterationCount,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _setIterationCount = setIterationCount;
        }

        public ForLoop(
            Func<T?, CancellationToken, Task<int>> setIterationCountAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) : base(name, stepSettings, steps)
        {
            _setIterationCountAsync = setIterationCountAsync;
        }

        public static ForLoop<T> Create(
            int iterations,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) => new ForLoop<T>(iterations, name, stepSettings, steps);

        public static ForLoop<T> Create(
            Func<T?, int> setIterationCount,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) => new ForLoop<T>(setIterationCount, name, stepSettings, steps);

        public static ForLoop<T> Create(
            Func<T?, CancellationToken, Task<int>> setIterationCountAsync,
            string? name = null,
            StepSettings? stepSettings = null,
            List<IStep<T>>? steps = null) => new ForLoop<T>(setIterationCountAsync, name, stepSettings, steps);

        protected override async Task ProcessAsync(T? state, CancellationToken cancellationToken)
        {
            if (_setIterationCount != null)
                _iterationCount = _setIterationCount(state);
            else if (_setIterationCountAsync != null)
                _iterationCount = await _setIterationCountAsync(state, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteExtensionProcessAsync(WorkflowState<T> workflowState, CancellationToken cancellationToken)
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