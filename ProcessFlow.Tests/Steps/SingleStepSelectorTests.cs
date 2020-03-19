using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps
{
    public class SingleStepSelectorTests
    {
        private WorkflowState<int> _workflowState;
        private int _originalWorfklowState;

        public SingleStepSelectorTests()
        {
            _workflowState = new WorkflowState<int>() { State = 0 };
            _originalWorfklowState = _workflowState.State;
        }
        
        [Fact]
        public async void SelectorSetsNext()
        {
            // Arrange
            var selector = new BaseSelector();
            var expectedOption = new BaseStep(name: "expectedOption");
            var option = new BaseStep(name: "option");
            var options = new List<Step<int>>() { expectedOption, option };
            selector.SetOptions(options);

            // Act
            var result = await selector.Execute(_workflowState);

            // Assert
            Assert.Single(result.WorkflowChain);
            Assert.Equal(_originalWorfklowState, result.State);
            Assert.Equal(selector.Next(), expectedOption);
        }
    }
}
