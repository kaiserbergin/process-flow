using System;
using System.Collections.Generic;
using System.Linq;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps
{
    public class SequencerTests
    {
        private WorkflowState<int> _workflowState;
        private int _originalWorfklowState;

        public SequencerTests()
        {
            _workflowState = new WorkflowState<int>() { State = 0 };
            _originalWorfklowState = _workflowState.State;
        }

        [Fact]
        public async void ProcessSequenceAfterAdd()
        {
            // Arrange
            var sequencer = new Sequencer<int>();
            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new BaseStep(name: firstStepName);
            var secondStep = new BaseStep(name: secondStepName);

            sequencer.AddStep(firstStep);
            sequencer.AddStep(secondStep);

            var expectedSequence = new List<Step<int>> { firstStep, secondStep };

            // Act
            var result = await sequencer.Execute(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState + 2, result.State);
            Assert.Equal(3, result.WorkflowChain.Count);
            Assert.True(expectedSequence.SequenceEqual(sequencer.GetSequence()));

            var sequencerStepLink = result.WorkflowChain.First.Value;
            var firstStepLink = result.WorkflowChain.First.Next.Value;
            var secondStepLink = result.WorkflowChain.Last.Value;

            Assert.Equal(sequencer.Id, sequencerStepLink.StepIdentifier);
            Assert.Equal(0.ToString(), sequencerStepLink.StateSnapshot);
            Assert.Equal(firstStep.Id, firstStepLink.StepIdentifier);
            Assert.Equal(1.ToString(), firstStepLink.StateSnapshot);
            Assert.Equal(secondStep.Id, secondStepLink.StepIdentifier);
            Assert.Equal(2.ToString(), secondStepLink.StateSnapshot);
        }

        [Fact]
        public async void ProcessSequenceAfterSet()
        {
            // Arrange
            var sequencer = new Sequencer<int>();
            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new BaseStep(name: firstStepName);
            var secondStep = new BaseStep(name: secondStepName);

            var expectedSequence = new List<Step<int>> { firstStep, secondStep };

            sequencer.SetSequence(expectedSequence);

            // Act
            var result = await sequencer.Execute(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState + 2, result.State);
            Assert.Equal(3, result.WorkflowChain.Count);
            Assert.True(expectedSequence.SequenceEqual(sequencer.GetSequence()));

            var sequencerStepLink = result.WorkflowChain.First.Value;
            var firstStepLink = result.WorkflowChain.First.Next.Value;
            var secondStepLink = result.WorkflowChain.Last.Value;

            Assert.Equal(sequencer.Id, sequencerStepLink.StepIdentifier);
            Assert.Equal(0.ToString(), sequencerStepLink.StateSnapshot);
            Assert.Equal(firstStep.Id, firstStepLink.StepIdentifier);
            Assert.Equal(1.ToString(), firstStepLink.StateSnapshot);
            Assert.Equal(secondStep.Id, secondStepLink.StepIdentifier);
            Assert.Equal(2.ToString(), secondStepLink.StateSnapshot);
        }

    }
}
