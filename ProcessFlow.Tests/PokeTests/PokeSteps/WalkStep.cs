using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class WalkStep : AbstractStep<PokeState>
    {
        public WalkStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task ProcessAsync(PokeState state, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}