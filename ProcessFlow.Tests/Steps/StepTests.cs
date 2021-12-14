using Xunit;
using ProcessFlow.Tests.TestUtils;
using ProcessFlow.Exceptions;
using ProcessFlow.Data;
using System.Linq;
using System;
using System.Threading.Tasks;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Base;

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
        public async void Create_WithSyncAction_Works()
        {
            // Arrange
            var step = Step<SimpleWorkflowState>.Create(state => state.MyInteger++);

            // Act
            var result = await step.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 1, result.State.MyInteger);
        }
        
        [Fact]
        public async void Create_WithAsyncAction_Works()
        {
            // Arrange
            var step = Step<SimpleWorkflowState>.Create(async state =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                state.MyInteger++;
            });

            // Act
            var result = await step.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 1, result.State.MyInteger);
        }
        
        [Fact]
        public async void ExceptionCaughtAndRethrownAsWorkflowActionException()
        {
            // Arrange
            var stepThatThrowsException = Step<SimpleWorkflowState>.Create(_ => throw new NotImplementedException());

            // Actsert
            var exeception = await Assert.ThrowsAsync<WorkflowActionException<SimpleWorkflowState>>(async () => await stepThatThrowsException.ExecuteAsync(_workflowState));

            Assert.True(exeception.InnerException is NotImplementedException);
            Assert.Equal(_workflowState.ToString(), exeception.WorkflowState.ToString());
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
        public void SetNextByType()
        {
            // Arrange
            var baseStepName = "base";
            var nextStepName = "next";

            var originStep = new BaseStep(name: baseStepName);
            var nextStep = new AnotherStepType(name: nextStepName);
            var optionsStep = new BaseSelector();

            // Act
            var actualNextStep = originStep.SetNext(nextStep);
            var actualOptionsStep = actualNextStep.SetNext(optionsStep);

            // Assert
            Assert.IsType<AnotherStepType>(actualNextStep);
            Assert.Equal(nextStep, originStep.Next());

            Assert.IsType<BaseSelector>(actualOptionsStep);
            Assert.Equal(optionsStep, nextStep.Next());
        }

        [Fact]
        public async void ExecuteCurrent()
        {
            // Arrange
            var baseStepName = "base";
            var originStep = new BaseStep(name: baseStepName);

            // Act
            var result = await originStep.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger + 1, result.State.MyInteger);
        }

        [Fact]
        public async void WorkflowChainUpdated()
        {
            // Arrange
            var baseStepName = "base";
            var settings = new StepSettings { TrackStateChanges = true };
            var originStep = new BaseStep(name: baseStepName, settings);

            // Act
            var result = await originStep.ExecuteAsync(_workflowState);
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
        public async void WorkflowChainUpdatedWithSnapshot()
        {
            // Arrange
            var baseStepName = "base";
            var originStep = new BaseStep(name: baseStepName);

            // Act
            var result = await originStep.ExecuteAsync(_workflowState);
            var link = result.WorkflowChain.First.Value;

            // Assert
            Assert.Single(result.WorkflowChain);
            Assert.Equal(baseStepName, link.StepName);
            Assert.Equal(originStep.Id, link.StepIdentifier);
            Assert.Equal(0, link.SequenceNumber);
            Assert.Null(link.GetUncompressedStateSnapshot<SimpleWorkflowState>());
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
            var settings = new StepSettings() { AutoProgress = true, TrackStateChanges = true };

            var originStep = new BaseStep(name: baseStepName, stepSettings: settings); 
            var nextStep = new BaseStep(name: nextStepName, stepSettings: settings);
            
            originStep.SetNext(nextStep);

            // Act
            var result = await originStep.ExecuteAsync(_workflowState);

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
        
        [Fact]
        public async void ExecuteAsync_WithAutoProgressSettingAndTerminate_StopsFlow()
        {
            // Arrange
            var nextStepName = "next";
            var settings = new StepSettings() { AutoProgress = true, TrackStateChanges = true };

            var originStep = Step<SimpleWorkflowState>.Create(((_, terminate) => terminate()), stepSettings: settings); 
            var nextStep = new BaseStep(name: nextStepName, stepSettings: settings);
            
            originStep.SetNext(nextStep);

            // Act
            var result = await originStep.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(_originalWorfklowState.MyInteger, result.State.MyInteger);
            Assert.Single(result.WorkflowChain);

            var firstLink = result.WorkflowChain.First.Value;

            Assert.Equal(originStep.Id, firstLink.StepIdentifier);
            Assert.Equal(0, firstLink.GetUncompressedStateSnapshot<SimpleWorkflowState>().MyInteger);
            Assert.Equal(0, result.State.MyInteger);

            Assert.Equal(StepActivityStages.Executing, firstLink.StepActivities.First().Activity);
            Assert.Equal(StepActivityStages.ExecutionTerminated, firstLink.StepActivities.Last().Activity);
        }
        
        [Fact]
        public async void DefaultStepSettings()
        {
            // Arrange
            var workflowState = new WorkflowState<SimpleWorkflowState> { State = new SimpleWorkflowState(), DefaultStepSettings = new StepSettings { AutoProgress = true, TrackStateChanges = true }};
            var originalWorkflowState = workflowState.State.DeepCopy();
            
            var baseStepName = "base"; 
            var nextStepName = "next";

            var originStep = new BaseStep(name: baseStepName); 
            var nextStep = new BaseStep(name: nextStepName);
            
            originStep.SetNext(nextStep);

            // Act
            var result = await originStep.ExecuteAsync(workflowState);

            // Assert
            Assert.Equal(originalWorkflowState.MyInteger + 2, result.State.MyInteger);
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
        
        [Fact]
        public async void DefaultStepSettings_OverridenByStep()
        {
            // Arrange
            var workflowState = new WorkflowState<SimpleWorkflowState> { State = new SimpleWorkflowState(), DefaultStepSettings = new StepSettings { AutoProgress = true, TrackStateChanges = true }};
            var originalWorkflowState = workflowState.State.DeepCopy();
            
            var baseStepName = "base"; 
            var nextStepName = "next";
            var unreachableStepName = "nope";

            var originStep = new BaseStep(name: baseStepName); 
            var nextStep = new BaseStep(name: nextStepName, stepSettings: new StepSettings { AutoProgress = false, TrackStateChanges = true});
            var unreachableStep = new BaseStep(name: unreachableStepName);
            
            originStep.SetNext(nextStep);

            // Act
            var result = await originStep.ExecuteAsync(workflowState);

            // Assert
            Assert.Equal(originalWorkflowState.MyInteger + 2, result.State.MyInteger);
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
