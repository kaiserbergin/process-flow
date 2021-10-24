using System.Threading;
using System.Threading.Tasks;
using Bogus;
using ProcessFlow.Data;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class CatchPokemonStep : LoopStep<PokeState>
    {
        public CatchPokemonStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task<PokeState> Process(PokeState state, CancellationToken cancellationToken = default)
        {
            if (state.PokeBallCount == 0 || state.EncounteredMon == null)
                Break();
        
            var faker = new Faker();
        
            state.PokeBallCount--;
        
            var throwModifier = faker.Random.Double(-.1d, .3d);
            var modifiedChance = throwModifier + state.EncounteredMon!.BaseCaptureChance;
        
            var chancey = faker.Random.Double();
        
            if (modifiedChance > chancey)
            {
                state.EncounteredMon.NickName = faker.Name.FirstName();
                state.MyPokemon.Add(state.EncounteredMon);
                state.EncounteredMon = null;
                
                Break();
            }

            if (CurrentIteration == 3)
                state.EncounteredMon = null; // Ran away!
        
            return Task.FromResult(state);
        }
    }
}