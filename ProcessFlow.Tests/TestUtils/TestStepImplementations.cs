using ProcessFlow.Data;
using ProcessFlow.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Steps.Loops;

namespace ProcessFlow.Tests.TestUtils
{
    class TestStepImplementations
    {
    }

    public class ExceptionalStep : Step<SimpleWorkflowState>
    {
        public ExceptionalStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseStep : Step<SimpleWorkflowState>
    {
        public BaseStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class AnotherStepType : Step<SimpleWorkflowState>
    {
        public AnotherStepType(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class LoopStep : LoopStep<SimpleWorkflowState>
    {
        public LoopStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class StopThatThrowsBreak : LoopStep<SimpleWorkflowState>
    {
        public StopThatThrowsBreak(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            Break();
            return Task.FromResult(state);
        }
    }

    public class StepThatThrowsContinue : LoopStep<SimpleWorkflowState>
    {
        public StepThatThrowsContinue(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            Continue();
            return Task.FromResult(state);
        }
    }

    public class BaseSelector : SingleStepSelector<SimpleWorkflowState>
    {
        protected override Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(options.First());
        }
    }

    public class AsyncStep : Step<SimpleWorkflowState>
    {
        private readonly int _delayMs;

        public AsyncStep(
            int delayMs,
            string name = null,
            StepSettings stepSettings = null,
            IClock clock = null) : base(name, stepSettings, clock)
        {
            _delayMs = delayMs;
        }

        protected override async Task<SimpleWorkflowState> Process(SimpleWorkflowState state, CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(_delayMs));

            if (state != null)
                state.MyInteger++;

            return state;
        }
    }
}