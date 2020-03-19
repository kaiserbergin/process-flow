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

    public class ExceptionalStep : Step<int>
    {
        protected override Task<int> Process(int state)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseStep : Step<int>
    {
        public BaseStep(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<int> Process(int state)
        {
            return Task.FromResult(state += 1);
        }
    }

    public class BaseSelector : SingleStepSelector<int>
    {
        protected override Task<Step<int>> Select(List<Step<int>> options, WorkflowState<int> workflowState)
        {
            return Task.FromResult(options.First());
        }
    }
}
