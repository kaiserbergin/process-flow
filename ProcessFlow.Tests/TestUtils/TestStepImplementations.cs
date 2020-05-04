using ProcessFlow.Data;
using ProcessFlow.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    class TestStepImplementations
    {
    }

    public class ExceptionalStep : Step<SimpleWorkflowState>
    {
        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseStep : Step<SimpleWorkflowState>
    {
        public BaseStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<SimpleWorkflowState> Process(SimpleWorkflowState state)
        {
            if (state != null)
                state.MyInteger++;
            return Task.FromResult(state);
        }
    }

    public class BaseSelector : SingleStepSelector<SimpleWorkflowState>
    {
        protected override Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState)
        {
            return Task.FromResult(options.First());
        }
    }
}
