using Xunit;
using ProcessFlow.Tests.TestUtils;
using ProcessFlow.Exceptions;
using ProcessFlow.Data;
using System.Linq;

namespace ProcessFlow.Tests.Steps
{
    public class StepTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;
        private SimpleWorkflowState _originalWorfklowState;

        public StepTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
            _originalWorfklowState = _workflowState.State.DeepCopy();
        }

        [Fact]
        public async void ExceptionCaughtAndRethrownAsWorkflowActionException()
        {
            // Arrange
            var stepThatThrowsException = new ExceptionalStep();

            // Actsert
            await Assert.ThrowsAsync<WorkflowActionException<SimpleWorkflowState>>(async () => await stepThatThrowsException.Execute(_workflowState));
        }

        [Fact]
        public void NextAndPreviousMethods()
        {
            // Arrange
            var baseStepName = "base";
            var previousStepName = "previous";
            var nextStepName = "next";

            var originStep = new BaseStep(name: baseStepName);
            var previousStep = new BaseStep(name: previousStepName);
            var nextStep = new BaseStep(name: nextStepName);

            // Act
            var actualNextStep = originStep.SetNext(nextStep);
            var actualpreviousStep = originStep.SetPrevious(previousStep);

            // Assert
            Assert.Equal(nextStep, actualNextStep);
            Assert.Equal(previousStep, actualpreviousStep);
            Assert.Equal(nextStep, originStep.Next());
            Assert.Equal(previousStep, originStep.Previous());
        }

        [Fact]
        public async void ExecuteCurrent()
        {
            // Arrange
            var baseStepName = "base";
            var originStep = new BaseStep(name: baseStepName);

            // Act
            var result = await originStep.Execute(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 1, result.State.MyInteger);
        }

        [Fact]
        public async void WorkflowChainUpdated()
        {
            // Arrange
            var baseStepName = "base";
            var originStep = new BaseStep(name: baseStepName);

            // Act
            var result = await originStep.Execute(_workflowState);
            var link = result.WorkflowChain.First.Value; 

            // Assert
            Assert.Single(result.WorkflowChain);
            Assert.Equal(baseStepName, link.StepName);
            Assert.Equal(originStep.Id, link.StepIdentifier);
            Assert.Equal(0, link.SequenceNumber);
            Assert.Equal(_originalWorfklowState.MyInteger + 1, link.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(StepActivityStages.Executing, link.StepActivities.First().Activity);
            Assert.Equal(StepActivityStages.ExecutionCompleted, link.StepActivities.Last().Activity);
        }

        [Fact]
        public void TerminateThrowsTerminateWorkflowException()
        {
            // Arrange
            var step = new BaseStep();

            // Actsert
            Assert.Throws<TerminateWorkflowException>(() => step.Terminate());
        }

        [Fact]
        public async void AutoProgressSetting()
        {
            // Arrange
            var baseStepName = "base"; 
            var nextStepName = "next";
            var settings = new StepSettings() { AutoProgress = true };

            var originStep = new BaseStep(name: baseStepName, stepSettings: settings); 
            var nextStep = new BaseStep(name: nextStepName, stepSettings: settings);
            
            originStep.SetNext(nextStep);

            // Act
            var result = await originStep.Execute(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 2, result.State.MyInteger);
            Assert.Equal(2, result.WorkflowChain.Count);

            var firstLink = result.WorkflowChain.First.Value;
            var secondLink = result.WorkflowChain.Last.Value;

            Assert.Equal(originStep.Id, firstLink.StepIdentifier);
            Assert.Equal(1, firstLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(nextStep.Id, secondLink.StepIdentifier);
            Assert.Equal(2, secondLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);

            Assert.Equal(StepActivityStages.Executing, firstLink.StepActivities.First().Activity);
            Assert.Equal(StepActivityStages.ExecutionCompleted, firstLink.StepActivities.Last().Activity);

            Assert.Equal(StepActivityStages.Executing, secondLink.StepActivities.First().Activity);
            Assert.Equal(StepActivityStages.ExecutionCompleted, secondLink.StepActivities.Last().Activity);
        }
    }
}
