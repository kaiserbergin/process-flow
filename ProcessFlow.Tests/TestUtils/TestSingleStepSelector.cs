using ProcessFlow.Data;
using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    public class TestStepSelector : ISingleStepSelector<int>
    {
        public Task<Step<int>> Select(List<Step<int>> options, WorkflowState<int> workflowState) => Task.FromResult(options.Last());
    }
}
