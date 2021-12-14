using System;
using System.Collections.Generic;
using System.Linq;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;
using ProcessFlow.Steps.Sequencers;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps
{
    public class SequencerTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;
        private SimpleWorkflowState _originalWorfklowState;

        public SequencerTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
            _originalWorfklowState = _workflowState.State.DeepCopy();
        }

        [Fact]
        public async void ProcessSequenceAfterAdd()
        {
            // Arrange
            var settings = new StepSettings { TrackStateChanges = true };

            var sequencer = new Sequencer<SimpleWorkflowState>(stepSettings: settings);
            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new BaseStep(name: firstStepName, settings);
            var secondStep = new BaseStep(name: secondStepName, settings);

            sequencer.AddStep(firstStep);
            sequencer.AddStep(secondStep);

            var expectedSequence = new List<AbstractStep<SimpleWorkflowState>> { firstStep, secondStep };

            // Act
            var result = await sequencer.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 2, result.State.MyInteger);
            Assert.Equal(3, result.WorkflowChain.Count);
            Assert.True(expectedSequence.SequenceEqual(sequencer.GetSequence()));

            var sequencerStepLink = result.WorkflowChain.First.Value;
            var firstStepLink = result.WorkflowChain.First.Next.Value;
            var secondStepLink = result.WorkflowChain.Last.Value;

            Assert.Equal(sequencer.Id, sequencerStepLink.StepIdentifier);

            var something = sequencerStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>();
            Assert.Equal(0, sequencerStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(firstStep.Id, firstStepLink.StepIdentifier);
            Assert.Equal(1, firstStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(secondStep.Id, secondStepLink.StepIdentifier);
            Assert.Equal(2, secondStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
        }

        [Fact]
        public async void ProcessSequenceAfterSet()
        {
            // Arrange
            var settings = new StepSettings { TrackStateChanges = true };
            var sequencer = new Sequencer<SimpleWorkflowState>(stepSettings: settings);

            var firstStepName = "first";
            var secondStepName = "second";

            var firstStep = new BaseStep(name: firstStepName, settings);
            var secondStep = new BaseStep(name: secondStepName, settings);

            var expectedSequence = new List<IStep<SimpleWorkflowState>> { firstStep, secondStep };

            sequencer.SetSequence(expectedSequence);

            // Act
            var result = await sequencer.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 2, result.State.MyInteger);
            Assert.Equal(3, result.WorkflowChain.Count);
            Assert.True(expectedSequence.SequenceEqual(sequencer.GetSequence()));

            var sequencerStepLink = result.WorkflowChain.First.Value;
            var firstStepLink = result.WorkflowChain.First.Next.Value;
            var secondStepLink = result.WorkflowChain.Last.Value;

            Assert.Equal(sequencer.Id, sequencerStepLink.StepIdentifier);
            Assert.Equal(0, sequencerStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(firstStep.Id, firstStepLink.StepIdentifier);
            Assert.Equal(1, firstStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(secondStep.Id, secondStepLink.StepIdentifier);
            Assert.Equal(2, secondStepLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
        }

        [Fact]
        public void Create_WithSteps_CreatesSuccessfully()
        {
            // Arrange
            const string firstStepName = "first";
            const string secondStepName = "second";
            var stepSettings = new StepSettings();

            var firstStep = new BaseStep(name: firstStepName, stepSettings: stepSettings);
            var secondStep = new BaseStep(name: secondStepName, stepSettings: stepSettings);

            var expectedOptions = new List<IStep<SimpleWorkflowState>> { firstStep, secondStep };

            // Act
            var result = Sequencer<SimpleWorkflowState>.Create(expectedOptions);

            // Assert
            Assert.True(expectedOptions.SequenceEqual(result.GetSequence()));
        }

    }
}
