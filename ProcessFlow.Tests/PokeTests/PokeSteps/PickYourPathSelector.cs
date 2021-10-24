using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class PickYourPathSelector : SingleStepSelector<PokeState>
    {
        public PickYourPathSelector(string name = null, StepSettings stepSettings = null) : base(name, stepSettings)
        {
        }

        protected override Task<Step<PokeState>> Select(List<Step<PokeState>> options, WorkflowState<PokeState> workflowState, CancellationToken cancellationToken)
        {
            var pokeState = workflowState.State;

            if (pokeState.DesiredPokemon == pokeState.MyPokemon.Count)
                return Task.FromResult(options.Single(o => o is ReleaseEmAllStep));

            if (pokeState.PokeBallCount < 1)
                return Task.FromResult(options.Single(o => o is GetMorePokeBallsStep));

            return Task.FromResult(options.Single(o => o is FindPokemonStep));
        }
    }
}