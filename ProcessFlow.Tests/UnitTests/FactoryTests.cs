using ProcessFlow.Data;
using ProcessFlow.Factory;
using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
using ProcessFlow.Tests.TestUtils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProcessFlow.Tests.UnitTests
{
    public class FactoryTests
    {
        [Fact]
        public void GetProcessorTest()
        {
            // Arrange
            var actions = GetProcessors();
            var expected = actions[1];
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(processors: actions);

            // Act
            var actual = factory.GetProcessor(expected.GetType());

            // Assert
            Assert.Equal(expected, actual);
            Assert.NotEqual(actions.Last(), actual);
        }

        [Fact]
        public void GetProcessorTypeArgTest()
        {
            // Arrange
            var actions = GetProcessors();
            var expected = actions[1];
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(processors: actions);

            // Act
            var actual = factory.GetProcessor<SecondProcessor>();

            // Assert
            Assert.Equal(expected, actual);
            Assert.NotEqual(actions.Last(), actual);
        }

        [Fact]
        public void GetStepSelectorTest()
        {
            // Arrange
            var actions = GetStepSelectors();
            var expected = actions[2];
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(stepSelectors: actions);

            // Act
            var actual = factory.GetStepSelector(expected.GetType());

            // Assert
            Assert.Equal(expected, actual);
            Assert.NotEqual(actions.Last(), actual);
        }

        [Fact]
        public void GetStepSelectorTyepArgTest()
        {
            // Arrange
            var actions = GetStepSelectors();
            var expected = actions[2];
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(stepSelectors: actions);

            // Act
            var actual = factory.GetStepSelector<ThirdStepSelector>();

            // Assert
            Assert.Equal(expected, actual);
            Assert.NotEqual(actions.Last(), actual);
        }

        [Fact]
        public void GetProcessorNotFoundTest()
        {
            // Arrange
            var actions = GetProcessors();
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(processors: actions);

            // Act
            var actual = factory.GetProcessor(1.GetType());

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetStepSelectorNotFoundTest()
        {
            // Arrange
            var actions = GetStepSelectors();
            var factory = new WorkflowActionFactory<SimpleWorkflowState>(stepSelectors: actions);

            // Act
            var actual = factory.GetStepSelector(1.GetType());

            // Assert
            Assert.Null(actual);
        }


        private List<IProcessor<SimpleWorkflowState>> GetProcessors() => new List<IProcessor<SimpleWorkflowState>>() { new FirstProcessor(), new SecondProcessor(), new ThirdProcessor(), new SecondProcessor() };

        private List<ISingleStepSelector<SimpleWorkflowState>> GetStepSelectors() => new List<ISingleStepSelector<SimpleWorkflowState>>() { new FirstStepSelector(), new SecondStepSelector(), new ThirdStepSelector(), new ThirdStepSelector() };
    }

    #region Classes used soley for the factory tests

    public class FirstProcessor : IProcessor<SimpleWorkflowState>
    {
        public Task<SimpleWorkflowState> Process(SimpleWorkflowState data) => Task.FromResult(data);
    }

    public class SecondProcessor : IProcessor<SimpleWorkflowState>
    {
        public Task<SimpleWorkflowState> Process(SimpleWorkflowState data) => Task.FromResult(data);
    }

    public class ThirdProcessor : IProcessor<SimpleWorkflowState>
    {
        public Task<SimpleWorkflowState> Process(SimpleWorkflowState data) => Task.FromResult(data);
    }

    public class FirstStepSelector : ISingleStepSelector<SimpleWorkflowState>
    {
        public Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    public class SecondStepSelector : ISingleStepSelector<SimpleWorkflowState>
    {
        public Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    public class ThirdStepSelector : ISingleStepSelector<SimpleWorkflowState>
    {
        public Task<Step<SimpleWorkflowState>> Select(List<Step<SimpleWorkflowState>> options, WorkflowState<SimpleWorkflowState> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    #endregion
}
