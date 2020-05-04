using ProcessFlow.Data;
using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    public class TestStepSelector : ISingleStepSelector<SimpleWorkflowState>
    {
        public Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState) => Task.FromResult(options.Last());
    }
}
