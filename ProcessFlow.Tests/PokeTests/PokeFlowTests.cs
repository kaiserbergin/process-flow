using System.Collections.Generic;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Tests.PokeTests.PokeData;
using ProcessFlow.Tests.PokeTests.PokeSteps;
using Xunit;

namespace ProcessFlow.Tests.PokeTests
{
    public class PokeFlowTests
    {
        /// <summary>
        /// Is this test lazy? Yes. Yes it is.
        /// </summary>
        [Fact]
        public async void Execute_WithValidState_DoesNotError()
        {
            // Arrange
            var pokeState = new PokeState();
            var catchAttemptsPerMon = 3;
            var settings = new StepSettings { AutoProgress = true };

            var findPokemonStep = new FindPokemonStep(stepSettings: settings);
            var catchPokemonStep = new CatchPokemonStep(stepSettings: settings);
            
            var catchLoop = new ForLoop<PokeState>(
                iterations: catchAttemptsPerMon,
                steps: new List<Step<PokeState>> { catchPokemonStep },
                stepSettings: settings);

            var getMorePokeBallsStep = new GetMorePokeBallsStep(stepSettings: settings);
            var releaseEamAllStep = new ReleaseEmAllStep(stepSettings: settings);

            var pickYourPathStep = new PickYourPathSelector(stepSettings: settings);
            pickYourPathStep.SetOptions(new List<Step<PokeState>> { findPokemonStep, getMorePokeBallsStep, releaseEamAllStep });

            var walkStep = new WalkStep(stepSettings: settings);
            var chewGumStep = new ChewGumStep(stepSettings: settings);

            findPokemonStep
                .SetNextStep(catchLoop)
                .SetNextStep(pickYourPathStep);

            getMorePokeBallsStep.SetNextStep(findPokemonStep);

            releaseEamAllStep.Fork(name: "someName", stepSettings: settings, walkStep, chewGumStep);

            // Actssert
            await findPokemonStep.Execute(new WorkflowState<PokeState> { State = pokeState });

            // Assert
        }
    }
}