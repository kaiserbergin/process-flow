using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class GetMorePokeBallsStep : AbstractStep<PokeState>
    {
        public GetMorePokeBallsStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task<PokeState> ProcessAsync(PokeState state, CancellationToken cancellationToken)
        {
            state.PokeBallCount = new Bogus.Randomizer().Number(6, 26);
            
            return Task.FromResult(state);
        }
    }
}