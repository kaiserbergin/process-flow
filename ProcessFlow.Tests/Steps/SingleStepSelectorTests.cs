using System.Collections.Generic;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps
{
    public class SingleStepSelectorTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;
        private SimpleWorkflowState _originalWorfklowState;

        public SingleStepSelectorTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>();
            _originalWorfklowState = _workflowState.State.DeepCopy();
        }
        
        [Fact]
        public async void SelectorSetsNext()
        {
            // Arrange
            var selector = new BaseSelector();
            var expectedOption = new BaseStep(name: "expectedOption");
            var option = new BaseStep(name: "option");
            var options = new List<IStep<SimpleWorkflowState>>() { expectedOption, option };
            selector.SetOptions(options);

            // Act
            var result = await selector.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(2, result.WorkflowChain.Count);
            Assert.Equal(_originalWorfklowState, result.State);
            Assert.Equal(result.WorkflowChain.Last.Value.StepName, expectedOption.Name);
        }
    }
}
