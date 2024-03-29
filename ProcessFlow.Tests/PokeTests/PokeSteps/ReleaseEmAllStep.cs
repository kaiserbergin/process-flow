using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.PokeTests.PokeData;

namespace ProcessFlow.Tests.PokeTests.PokeSteps
{
    public class ReleaseEmAllStep : AbstractStep<PokeState>
    {
        public ReleaseEmAllStep(string name = null, StepSettings stepSettings = null, IClock clock = null) : base(name, stepSettings, clock)
        {
        }

        protected override Task ProcessAsync(PokeState state, CancellationToken cancellationToken)
        {
            for (int i = state.MyPokemon.Count - 1; i >= 0; i--)
            {
                state.MyPokemon[i].StateName();
                state.MyPokemon.RemoveAt(i);
            }

            return Task.CompletedTask;
        }
    }
}