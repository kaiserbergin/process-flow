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
    public class AbstractStepDictionarySelectorTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public AbstractStepDictionarySelectorTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
        }

        [Fact]
        public async void ExecuteAsync_ForSpecificSteps_ExecutesDesiredSteps()
        {
            // Arrange
            var dictionarySelector = new DictionarySelector();
            var expectedIntegerValue = 2;
            var expectedStepsExecutedCount = 3;
            var expectedStepsExecuted = new[] { nameof(DictionarySelector), StepConstants.FIRST_STEP_NAME, StepConstants.THIRD_STEP_NAME };
            
            // Act
            var result = await dictionarySelector.ExecuteAsync(_workflowState);
            
            // Assert
            Assert.Equal(expectedIntegerValue, result.State.MyInteger);
            Assert.Equal(expectedStepsExecutedCount, result.WorkflowChain.Count);
            Assert.True(expectedStepsExecuted.SequenceEqual(result.WorkflowChain.ToList().Select(x => x.StepName)));
        }
    }

    public class DictionarySelector : AbstractStepDictionarySelector<SimpleWorkflowState>
    {
        public DictionarySelector()
        {
            var firstStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.FIRST_STEP_NAME);
            var secondStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.SECOND_STEP_NAME);
            var thirdStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.THIRD_STEP_NAME);

            var options = new Dictionary<string, IStep<SimpleWorkflowState>>
            {
                { StepConstants.FIRST_STEP_NAME, firstStep },
                { StepConstants.SECOND_STEP_NAME, secondStep },
                { StepConstants.THIRD_STEP_NAME, thirdStep },
            };

            SetOptions(options);
        }

        protected override Task<List<IStep<SimpleWorkflowState>>> SelectAsync(
            Dictionary<string, IStep<SimpleWorkflowState>> options,
            WorkflowState<SimpleWorkflowState> workflowState,
            CancellationToken? cancellationToken = default) =>
            Task.FromResult(new List<IStep<SimpleWorkflowState>> { options[StepConstants.FIRST_STEP_NAME], options[StepConstants.THIRD_STEP_NAME] });
    }
}