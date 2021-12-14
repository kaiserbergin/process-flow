using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Steps.Selectors;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class PickYourPathSelector : AbstractStepListSelector<PokeState>
    {
        public PickYourPathSelector(List<IStep<PokeState>> options = null, string name = null, StepSettings stepSettings = null) : base(options, name, stepSettings)
        {
        }

        protected override Task<List<IStep<PokeState>>> SelectAsync(List<IStep<PokeState>> options, WorkflowState<PokeState> workflowState, CancellationToken? cancellationToken = default)
        {
            var pokeState = workflowState.State;

            if (pokeState.DesiredPokemon == pokeState.MyPokemon.Count)
                return Task.FromResult(new List<IStep<PokeState>> { options.Single(o => o is ReleaseEmAllStep) });

            if (pokeState.PokeBallCount < 1)
                return Task.FromResult(new List<IStep<PokeState>> {options.Single(o => o is GetMorePokeBallsStep) });

            return Task.FromResult(new List<IStep<PokeState>> { options.Single(o => o is FindPokemonStep) });
        }
    }
}