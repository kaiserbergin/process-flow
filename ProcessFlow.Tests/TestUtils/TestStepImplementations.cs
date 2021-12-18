using ProcessFlow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Steps.Base;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Steps.Selectors;

namespace ProcessFlow.Tests.TestUtils
{
    public class ExceptionalStep : AbstractStep<SimpleWorkflowState>
    {
        public ExceptionalStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseStep : AbstractStep<SimpleWorkflowState>
    {
        public BaseStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;

            return Task.CompletedTask;
        }
    }

    public class AnotherStepType : AbstractStep<SimpleWorkflowState>
    {
        public AnotherStepType(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;
            
            return Task.CompletedTask;
        }
    }

    public class LoopStep : AbstractLoopStep<SimpleWorkflowState>
    {
        public LoopStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            if (state != null)
                state.MyInteger++;

            return Task.CompletedTask;
        }
    }

    public class StopThatThrowsBreak : AbstractLoopStep<SimpleWorkflowState>
    {
        public StopThatThrowsBreak(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            Break();

            return Task.CompletedTask;
        }
    }

    public class StepThatThrowsContinue : AbstractLoopStep<SimpleWorkflowState>
    {
        public StepThatThrowsContinue(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            Continue();

            return Task.CompletedTask;
        }
    }

    public class BaseSelector : AbstractStepListSelector<SimpleWorkflowState>
    {
        protected override Task<List<IStep<SimpleWorkflowState>>> SelectAsync(
            WorkflowState<SimpleWorkflowState> workflowState, 
            List<IStep<SimpleWorkflowState>> options, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<IStep<SimpleWorkflowState>> { options.First() });
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

        protected override async Task ProcessAsync(SimpleWorkflowState state, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(_delayMs));

            if (state != null)
                state.MyInteger++;
        }
    }
}