using ProcessFlow.Data;
using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
using ProcessFlow.Tests.TestUtils;
using System.Collections.Generic;
using Xunit;

namespace ProcessFlow.Tests.UnitTests
{
    public class SelectorTests
    {
        private IProcessor<SimpleWorkflowState> _processor;
        private ISingleStepSelector<SimpleWorkflowState> _stepSelector;

        public SelectorTests()
        {
            _processor = new TestProcessor();
            _stepSelector = new TestStepSelector();
        }

        [Fact]
        public void TestSelectorNoOptions()
        {
            // Arrange
            var selector = new Selector<SimpleWorkflowState>();

            // Act
            var result = selector.Options();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void TestSelectorSetOptions()
        {
            // Arrange
            var selector = new Selector<SimpleWorkflowState>();
            var options = new List<Step<SimpleWorkflowState>>() { new Step<SimpleWorkflowState>() };

            // Act
            selector.SetOptions(options);
            var result = selector.Options();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async void TestSelectorSetOptionsInConstructorAndSelect()
        {
            // Arrange
            var selector = new Selector<SimpleWorkflowState>(stepSettings: new StepSettings() { AutoProgress = true }, stepSelector: _stepSelector);
            var selectedStepName = "selectedStepName";
            var options = new List<Step<SimpleWorkflowState>>() { new Step<SimpleWorkflowState>(), new Step<SimpleWorkflowState>(name: selectedStepName, processor: _processor) };
            var workflowState = new WorkflowState<SimpleWorkflowState>();

            // Act
            selector.SetOptions(options);
            await selector.Process(workflowState);

            // Assert
            Assert.Equal(2, workflowState.WorkflowChain.Count);
            Assert.Equal(selectedStepName, workflowState.WorkflowChain.Last.Value.StepName);
        }

        [Fact]
        public async void TestSelectorSetOptionsAndSelect()
        {
            // Arrange
            var selector = new Selector<SimpleWorkflowState>(stepSettings: new StepSettings() { AutoProgress = true });
            selector.SetStepSelector(_stepSelector);
            var selectedStepName = "selectedStepName";
            var options = new List<Step<SimpleWorkflowState>>() { new Step<SimpleWorkflowState>(), new Step<SimpleWorkflowState>(name: selectedStepName, processor: _processor) };
            var workflowState = new WorkflowState<SimpleWorkflowState>();

            // Act
            selector.SetOptions(options);
            await selector.Process(workflowState);

            // Assert
            Assert.Equal(2, workflowState.WorkflowChain.Count);
            Assert.Equal(selectedStepName, workflowState.WorkflowChain.Last.Value.StepName);
        }
    }
}
