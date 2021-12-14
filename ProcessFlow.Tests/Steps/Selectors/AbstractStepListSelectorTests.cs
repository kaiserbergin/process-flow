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
    public class AbstractSteListSelectorTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public AbstractSteListSelectorTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
        }

        [Fact]
        public async void ExecuteAsync_ForSpecificSteps_ExecutesDesiredSteps()
        {
            // Arrange
            var listSelector = new ListSelector();
            var expectedIntegerValue = 2;
            var expectedStepsExecutedCount = 3;
            var expectedStepsExecuted = new[] { nameof(ListSelector), StepConstants.FIRST_STEP_NAME, StepConstants.THIRD_STEP_NAME };
            
            // Act
            var result = await listSelector.ExecuteAsync(_workflowState);
            
            // Assert
            Assert.Equal(expectedIntegerValue, result.State.MyInteger);
            Assert.Equal(expectedStepsExecutedCount, result.WorkflowChain.Count);
            Assert.True(expectedStepsExecuted.SequenceEqual(result.WorkflowChain.ToList().Select(x => x.StepName)));
        }
    }
    
    public class ListSelector : AbstractStepListSelector<SimpleWorkflowState>
    {
        public ListSelector()
        {
            var firstStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.FIRST_STEP_NAME);
            var secondStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.SECOND_STEP_NAME);
            var thirdStep = Step<SimpleWorkflowState>.Create(state => state.MyInteger++, name: StepConstants.THIRD_STEP_NAME);

            var options = new List<IStep<SimpleWorkflowState>> { firstStep, secondStep, thirdStep };

            SetOptions(options);
        }

        protected override Task<List<IStep<SimpleWorkflowState>>> SelectAsync(
            List<IStep<SimpleWorkflowState>> options, 
            WorkflowState<SimpleWorkflowState> workflowState, 
            CancellationToken? cancellationToken = default) => Task.FromResult(new List<IStep<SimpleWorkflowState>> { options.First(), options.Last() });
    }
}