using System.Linq;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps
{
    public class ForkTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public ForkTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState> { State = new SimpleWorkflowState() };
        }

        [Fact]
        public async void ProcessFork()
        {
            // Arrange
            var settings = new StepSettings { TrackStateChanges = true, AutoProgress = true };

            const string firstStepName = "first";
            const string forkStepName = "fork";
            const string secondStepName = "second";
            const string thirdStepName = "third";
            const string fourthStepName = "fourth";

            var firstBaseStep = new BaseStep(firstStepName, settings);
            var secondStepAsync = new AsyncStep(500, secondStepName);
            var thirdStepAsync = new AsyncStep(0, thirdStepName);
            var fourthStepAsync = new AsyncStep(200, fourthStepName);

            firstBaseStep.Fork(
                name: forkStepName,
                stepSettings: settings,
                secondStepAsync,
                thirdStepAsync,
                fourthStepAsync);

            var expectedExecutionStarted = new[] { firstStepName, forkStepName, secondStepName, thirdStepName, fourthStepName };
            var expectedExecutionCompletionOrder = new[] { firstStepName, thirdStepName, fourthStepName, secondStepName, forkStepName };

            // Act
            var result = await firstBaseStep.ExecuteAsync(_workflowState);

            var orderedByStarted = result.WorkflowChain.ToList().Select(x => x.StepName).ToArray();
            var orderedByCompletion = result.WorkflowChain.ToList()
                .OrderBy(x => x.StepActivities.FirstOrDefault(y => y.Activity == StepActivityStages.ExecutionCompleted).DateTimeOffset)
                .Select(x => x.StepName)
                .ToArray();

            // Assert
            Assert.True(expectedExecutionStarted.SequenceEqual(orderedByStarted));
            Assert.True(expectedExecutionCompletionOrder.SequenceEqual(orderedByCompletion));
        }

        [Fact]
        public async void ProcessForkWithException()
        {
            // Arrange
            var settings = new StepSettings { TrackStateChanges = true, AutoProgress = true };

            const string firstStepName = "first";
            const string forkStepName = "fork";
            const string secondStepName = "second";
            const string exceptionalStepName = "exceptionalStep";
            const string fourthStepName = "fourth";

            var firstBaseStep = new BaseStep(firstStepName, settings);
            var secondStepAsync = new AsyncStep(500, secondStepName);
            var exceptionalStep = new ExceptionalStep(exceptionalStepName);
            var fourthStepAsync = new AsyncStep(200, fourthStepName);

            firstBaseStep.Fork(
                name: forkStepName,
                stepSettings: settings,
                secondStepAsync,
                exceptionalStep,
                fourthStepAsync);

            var expectedExecutionStarted = new[] { firstStepName, forkStepName, secondStepName, exceptionalStepName, fourthStepName };
            var expectedExecutionCompletionOrder = new[] { firstStepName, fourthStepName, secondStepName };
            var expectedFailedOrder = new[] { forkStepName, exceptionalStepName };

            // Actssert
            try
            {
                await firstBaseStep.ExecuteAsync(_workflowState);
            }
            catch (WorkflowActionException<SimpleWorkflowState> ex)
            {
                var result = ex.WorkflowState;
                
                var orderedByStarted = result.WorkflowChain.ToList().Select(x => x.StepName).ToArray();
                var orderedByCompletion = result.WorkflowChain.ToList()
                    .Where(x => x.StepActivities.Any(y => y.Activity == StepActivityStages.ExecutionCompleted))
                    .OrderBy(x => x.StepActivities.FirstOrDefault(y => y.Activity == StepActivityStages.ExecutionCompleted).DateTimeOffset)
                    .Select(x => x.StepName)
                    .ToArray();
                var failedStepNames = result.WorkflowChain.ToList()
                    .Where(x => x.StepActivities.Any(y => y.Activity == StepActivityStages.ExecutionFailed))
                    .Select(x => x.StepName)
                    .ToArray();

                // Assert
                Assert.True(expectedExecutionStarted.SequenceEqual(orderedByStarted));
                Assert.True(expectedExecutionCompletionOrder.SequenceEqual(orderedByCompletion));
                Assert.True(expectedFailedOrder.SequenceEqual(failedStepNames));
            }
        }
    }
}