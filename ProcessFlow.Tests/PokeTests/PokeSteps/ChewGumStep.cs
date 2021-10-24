using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class ChewGumStep : Step<PokeState>
    {
        public ChewGumStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task<PokeState> ProcessAsync(PokeState state, CancellationToken cancellationToken)
        {
            return Task.FromResult(state);
        }
    }
}