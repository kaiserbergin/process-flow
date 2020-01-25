using ProcessFlow.Interfaces;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    public class TestProcessor : IProcessor<int>
    {
        public Task<int> Process(int data) => Task.FromResult(data + 1);
    }
}
