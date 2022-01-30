using System;
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
    public class StepSelectorTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public StepSelectorTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
        }

        [Fact]
        public async void Create_WithAsyncSelect_ExecutesSuccessfully()
        {
            // Arrange
            const int expectedIntegerValue = 1;
            const int expectedStepsExecutedCount = 2;
            var expectedStepsExecuted = new[] { "StepSelector`1", StepConstants.SECOND_STEP_NAME };

            var expectedOptions = OptionsGenerator.GetDictionaryOptions();
            
            var stepSelector = StepSelector<SimpleWorkflowState>.Create(
                (_, options, _, _) => Task.FromResult(new List<IStep<SimpleWorkflowState>> { options[StepConstants.SECOND_STEP_NAME] }),
                expectedOptions);

            // Act
            var result = await stepSelector.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(expectedIntegerValue, result.State.MyInteger);
            Assert.Equal(expectedStepsExecutedCount, result.WorkflowChain.Count);
            Assert.True(expectedStepsExecuted.SequenceEqual(result.WorkflowChain.ToList().Select(x => x.StepName)));
        }

        [Fact]
        public async void Create_WithSyncSelect_ExecutesSuccessfully()
        {
            // Arrange
            const int expectedIntegerValue = 1;
            const int expectedStepsExecutedCount = 2;
            var expectedStepsExecuted = new[] { "StepSelector`1", StepConstants.SECOND_STEP_NAME };

            var options = OptionsGenerator.GetDictionaryOptions();
            
            var stepSelector = StepSelector<SimpleWorkflowState>.Create(
                (_, stepOptions, _) => new List<IStep<SimpleWorkflowState>> { stepOptions[StepConstants.SECOND_STEP_NAME] }, 
                options);

            // Act
            var result = await stepSelector.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(expectedIntegerValue, result.State.MyInteger);
            Assert.Equal(expectedStepsExecutedCount, result.WorkflowChain.Count);
            Assert.True(expectedStepsExecuted.SequenceEqual(result.WorkflowChain.ToList().Select(x => x.StepName)));
        }
    }
}