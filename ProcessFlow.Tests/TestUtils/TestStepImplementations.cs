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

    public class ExceptionalStep : AbstractStep<SimpleWorkflowState>
    {
        public ExceptionalStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseStep : AbstractStep<SimpleWorkflowState>
    {
        public BaseStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class AnotherStepType : AbstractStep<SimpleWorkflowState>
    {
        public AnotherStepType(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class LoopStep : AbstractLoopStep<SimpleWorkflowState>
    {
        public LoopStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class StopThatThrowsBreak : AbstractLoopStep<SimpleWorkflowState>
    {
        public StopThatThrowsBreak(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            Break();
            return Task.FromResult(state);
        }
    }

    public class StepThatThrowsContinue : AbstractLoopStep<SimpleWorkflowState>
    {
        public StepThatThrowsContinue(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            Continue();
            return Task.FromResult(state);
        }
    }

    public class BaseSelector : SingleStepSelector<SimpleWorkflowState>
    {
        protected override Task<AbstractStep<SimpleWorkflowState>> SelectAsync(List<AbstractStep<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState, CancellationToken cancellationToken)
        {
            return Task.FromResult(options.First());
        }
    }

    public class AsyncStep : AbstractStep<SimpleWorkflowState>
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

        protected override async Task<SimpleWorkflowState> ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(_delayMs));

            if (state != null)
                state.MyInteger++;

            return state;
        }
    }
}