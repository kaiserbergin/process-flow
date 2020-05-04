using ProcessFlow.Interfaces;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    public class TestProcessor : IProcessor<SimpleWorkflowState>
    {
        public Task<SimpleWorkflowState> Process(SimpleWorkflowState data)
        {
            if (data != null)
                data.MyInteger++;
            return Task.FromResult(data);
        }
    }
}
