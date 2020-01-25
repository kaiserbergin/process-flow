using ProcessFlow.Data;
using ProcessFlow.Factory;
using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
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
            var factory = new WorkflowActionFactory<string>(processors: actions);

            // Act
            var actual = factory.GetProcessor(expected.GetType());

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
            var factory = new WorkflowActionFactory<string>(stepSelectors: actions);

            // Act
            var actual = factory.GetStepSelector(expected.GetType());

            // Assert
            Assert.Equal(expected, actual);
            Assert.NotEqual(actions.Last(), actual);
        }

        [Fact]
        public void GetProcessorNotFoundTest()
        {
            // Arrange
            var actions = GetProcessors();
            var factory = new WorkflowActionFactory<string>(processors: actions);

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
            var factory = new WorkflowActionFactory<string>(stepSelectors: actions);

            // Act
            var actual = factory.GetStepSelector(1.GetType());

            // Assert
            Assert.Null(actual);
        }


        private List<IProcessor<string>> GetProcessors() => new List<IProcessor<string>>() { new FirstProcessor(), new SecondProcessor(), new ThirdProcessor(), new SecondProcessor() };

        private List<ISingleStepSelector<string>> GetStepSelectors() => new List<ISingleStepSelector<string>>() { new FirstStepSelector(), new SecondStepSelector(), new ThirdStepSelector(), new ThirdStepSelector() };
    }

    #region Classes used soley for the factory tests

    public class FirstProcessor : IProcessor<string>
    {
        public Task<string> Process(string data) => Task.FromResult(data);
    }

    public class SecondProcessor : IProcessor<string>
    {
        public Task<string> Process(string data) => Task.FromResult(data);
    }

    public class ThirdProcessor : IProcessor<string>
    {
        public Task<string> Process(string data) => Task.FromResult(data);
    }

    public class FirstStepSelector : ISingleStepSelector<string>
    {
        public Task<Step<string>> Select(List<Step<string>> options, WorkflowState<string> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    public class SecondStepSelector : ISingleStepSelector<string>
    {
        public Task<Step<string>> Select(List<Step<string>> options, WorkflowState<string> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    public class ThirdStepSelector : ISingleStepSelector<string>
    {
        public Task<Step<string>> Select(List<Step<string>> options, WorkflowState<string> workflowState) => Task.FromResult(options.FirstOrDefault());
    }

    #endregion
}
