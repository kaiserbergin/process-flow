using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;
using ProcessFlow.Steps.Selectors;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps.Selectors
{
    public class AbstractStepSelectorTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public AbstractStepSelectorTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
        }

        [Fact]
        public async void ExecuteAsync_ForSpecificSteps_ExecutesDesiredSteps()
        {
            // Arrange
            var dictionarySelector = new Selector();
            var expectedIntegerValue = 2;
            var expectedStepsExecutedCount = 3;
            var expectedStepsExecuted = new[] { nameof(Selector), StepConstants.FIRST_STEP_NAME, StepConstants.THIRD_STEP_NAME };
            
            // Act
            var result = await dictionarySelector.ExecuteAsync(_workflowState);
            
            // Assert
            Assert.Equal(expectedIntegerValue, result.State.MyInteger);
            Assert.Equal(expectedStepsExecutedCount, result.WorkflowChain.Count);
            Assert.True(expectedStepsExecuted.SequenceEqual(result.WorkflowChain.ToList().Select(x => x.StepName)));
        }
        
        [Fact]
        public void Options_ReturnsSuccessfully()
        {
            // Arrange
            var selector = new Selector();

            // Act
            var result = selector.Options();

            // Assert
            Assert.IsType<Dictionary<string, IStep<SimpleWorkflowState>>>(result);
            Assert.Equal(OptionsGenerator.GetDictionaryOptions().Count, result.Count);
        }
    }

    public class Selector : AbstractStepSelector<SimpleWorkflowState>
    {
        public Selector()
        {
            SetOptions(OptionsGenerator.GetDictionaryOptions());
        }

        protected override Task<List<IStep<SimpleWorkflowState>>> SelectAsync(
            WorkflowState<SimpleWorkflowState> workflowState,
            Dictionary<string, IStep<SimpleWorkflowState>> options,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new List<IStep<SimpleWorkflowState>> { options[StepConstants.FIRST_STEP_NAME], options[StepConstants.THIRD_STEP_NAME] });
    }
}