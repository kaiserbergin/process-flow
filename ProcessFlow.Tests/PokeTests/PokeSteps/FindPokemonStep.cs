using System.Threading;
using System.Threading.Tasks;
using Bogus;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class FindPokemonStep : AbstractStep<PokeState>
    {
        public FindPokemonStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task<PokeState> ProcessAsync(PokeState state, CancellationToken cancellationToken)
        {
            state.EncounteredMon = new Faker<Pokemon>()
                .StrictMode(false)
                .RuleFor(p => p.Level, f => f.Random.Number(1, 5))
                .RuleFor(p => p.PokeSpecies, f => f.PickRandom<PokeSpecies>())
                .RuleFor(p => p.BaseCaptureChance, f => f.Random.Double(.1d, .5d))
                .Generate();

            return Task.FromResult(state);
        }
    }
}