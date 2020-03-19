using ProcessFlow.Data;
using ProcessFlow.Flow;
using ProcessFlow.Tests.TestUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using ProcessFlow.Interfaces;
using ProcessFlow.Exceptions;
using System.Text.Json;

namespace ProcessFlow.Tests.UnitTests
{
    public class ProcessorTests
    {
        private TestProcessor _testProcessor;
        private Mock<IClock> _mockClock;

        public ProcessorTests()
        {
            _testProcessor = new TestProcessor();
            var nowish = DateTimeOffset.UtcNow;
            _mockClock = new Mock<IClock>();
            _mockClock.Setup(x => x.UtcNow()).Returns(nowish);
        }

        [Fact]
        public void TestSetNextAndNextWhenNextExists()
        {
            // Arrange
            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new Step<int>(name: firstStepName);
            var secondStep = new Step<int>(name: secondStepName);

            // Act
            var createdNext = firstStep.SetNext(secondStep);
            var next = firstStep.Next();

            // Assert
            Assert.Equal(secondStepName, createdNext.Name);
            Assert.Equal(secondStepName, next.Name);
        }

        [Fact]
        public void TestNextWhenNextIsNull()
        {
            // Arrange
            var firstStepName = "first";
            var firstStep = new Step<int>(name: firstStepName);

            // Act
            var next = firstStep.Next();

            // Assert
            Assert.Null(next);
        }

        [Fact]
        public void TestSetPreviousAndPreviousWhenPreviousExists()
        {
            // Arrange
            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new Step<int>(name: firstStepName);
            var secondStep = new Step<int>(secondStepName);

            // Act
            var createdPrevious = firstStep.SetPrevious(secondStep);
            var previous = firstStep.Previous();

            // Assert
            Assert.Equal(secondStepName, createdPrevious.Name);
            Assert.Equal(secondStepName, previous.Name);
        }

        [Fact]
        public void TestPreviousWhenPreviousIsNull()
        {
            // Arrange
            var firstStepName = "first";
            var firstStep = new Step<int>(name: firstStepName);

            // Act
            var next = firstStep.Previous();

            // Assert
            Assert.Null(next);
        }

        [Fact]
        public async void TestProcessNoData()
        {
            // Arrange
            var stepName = "processorName";
            var step = new Step<int>(name: stepName, processor: _testProcessor);
            var state = new WorkflowState<int>();

            // Act
            var result = await step.Process(state);
            var chain = result.WorkflowChain;
            var firstLink = chain.First();

            // Assert
            Assert.Single(chain);

            Assert.Equal(stepName, firstLink.StepName);
            Assert.Equal(0, firstLink.SequenceNumber);
            Assert.Equal(1, int.Parse(firstLink.StateSnapshot));
        }

        [Fact]
        public async void TestProcessWithData()
        {
            // Arrange
            var stepName = "processorName";
            var step = new Step<int>(name: stepName, processor: _testProcessor);

            var link1 = new WorkflowChainLink()
            {
                StateSnapshot = 5.ToString(),
                SequenceNumber = 0,
                StepName = "first"
            };
            var link2 = new WorkflowChainLink()
            {
                StateSnapshot = 6.ToString(),
                SequenceNumber = 1,
                StepName = "second"
            };

            var workflowChain = new LinkedList<WorkflowChainLink>();
            workflowChain.AddLast(link1);
            workflowChain.AddLast(link2);

            var workflowState = new WorkflowState<int>
            {
                State = 6,
                WorkflowChain = workflowChain
            };

            // Act
            var result = await step.Process(workflowState);
            var chain = result.WorkflowChain;
            var fistLink = chain.First();
            var secondLink = chain.First.Next.Value;
            var newLink = chain.Last();

            // Assert
            Assert.Equal(3, chain.Count);

            Assert.Equal(link1, fistLink);
            Assert.Equal(link2, secondLink);

            Assert.Equal(stepName, newLink.StepName);
            Assert.Equal(link2.SequenceNumber + 1, newLink.SequenceNumber);
            Assert.Equal(7.ToString(), newLink.StateSnapshot);
            Assert.True(Guid.TryParse(newLink.StepIdentifier, out var guid));
            Assert.IsType<Guid>(guid);
            Assert.Equal(7, result.State);
        }

        [Fact]
        public async void TestWorfklowException()
        {
            // Arrange
            var stepName = "processorName";
            var mockProcessor = new Mock<IProcessor<int>>();

            mockProcessor.Setup(x => x.Process(It.IsAny<int>()))
                .Throws(new Exception());

            var step = new Step<int>(name: stepName, processor: mockProcessor.Object);

            var link1 = new WorkflowChainLink()
            {
                StateSnapshot = 5.ToString(),
                SequenceNumber = 0,
                StepName = "first"
            };

            var workflowChain = new LinkedList<WorkflowChainLink>();
            workflowChain.AddLast(link1);

            var workflowState = new WorkflowState<int>
            {
                State = 1,
                WorkflowChain = workflowChain
            };

            // Assert
            var exception = await Assert.ThrowsAsync<WorkflowActionException<int>>(async () => await step.Process(workflowState));
            Assert.Equal(workflowState, exception.WorkflowState);
        }

        [Fact]
        public async void TestAutomatedFlow()
        {
            // Arrange
            var firstStepName = "firstStepName";
            var secondStepName = "secondStepName";
            var thirdStepName = "thirdStepName";

            var firstStepId = "firstStepId";
            var secondStepId = "secondStepId";
            var thirdStepId = "thirdStepId";

            var stepSettings = new StepSettings() { AutoProgress = true };

            var firstStep = new Step<int>(name: firstStepName, id: firstStepId, stepSettings: stepSettings, processor: _testProcessor, clock: _mockClock.Object);
            var secondStep = new Step<int>(name: secondStepName, id: secondStepId, stepSettings: stepSettings, processor: _testProcessor, clock: _mockClock.Object);
            var thirdStep = new Step<int>(name: thirdStepName, id: thirdStepId, stepSettings: stepSettings, processor: _testProcessor, clock: _mockClock.Object);

            firstStep
                .SetNext(secondStep)
                .SetNext(thirdStep);

            var workflowState = new WorkflowState<int>() { State = 0 };

            var expectedWorkflowChain = new LinkedList<WorkflowChainLink>();

            var firstexpectedLink = new WorkflowChainLink()
            {
                StateSnapshot = 1.ToString(),
                StepIdentifier = firstStepId,
                StepName = firstStepName,
                SequenceNumber = 0,
                StepActivities = new List<StepActivity>() { new StepActivity(StepActivityStages.Executing, clock: _mockClock.Object), new StepActivity(StepActivityStages.ExecutionCompleted, clock: _mockClock.Object) }
            };
            var secondExpectedLink = new WorkflowChainLink()
            {
                StateSnapshot = 2.ToString(),
                StepIdentifier = secondStepId,
                StepName = secondStepName,
                SequenceNumber = 1,
                StepActivities = new List<StepActivity>() { new StepActivity(StepActivityStages.Executing, clock: _mockClock.Object), new StepActivity(StepActivityStages.ExecutionCompleted, clock: _mockClock.Object) }
            };
            var thirdExpectedLink = new WorkflowChainLink()
            {
                StateSnapshot = 3.ToString(),
                StepIdentifier = thirdStepId,
                StepName = thirdStepName,
                SequenceNumber = 2,
                StepActivities = new List<StepActivity>() { new StepActivity(StepActivityStages.Executing, clock: _mockClock.Object), new StepActivity(StepActivityStages.ExecutionCompleted, clock: _mockClock.Object) }
            };

            expectedWorkflowChain.AddLast(firstexpectedLink);
            expectedWorkflowChain.AddLast(secondExpectedLink);
            expectedWorkflowChain.AddLast(thirdExpectedLink);

            var expectedWorkflowState = new WorkflowState<int>()
            {
                State = 3,
                WorkflowChain = expectedWorkflowChain
            };

            // Act
            var result = await firstStep.Process(workflowState);

            // Assert
            Assert.Equal(expectedWorkflowState.State, result.State);

            var currentExpectedLink = expectedWorkflowChain.First;

            foreach (var resultLink in result.WorkflowChain)
            {
                Assert.Equal(JsonSerializer.Serialize(currentExpectedLink.Value), JsonSerializer.Serialize(resultLink));

                currentExpectedLink = currentExpectedLink.Next;
            }
        }
    }
}
