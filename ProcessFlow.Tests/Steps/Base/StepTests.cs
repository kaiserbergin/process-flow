using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps.Base
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
        public async void Create_WithAsyncAndTerminate_Terminates()
        {
            // Arrange
            var step = Step<SimpleWorkflowState>.Create(async (_, terminate) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                terminate();
            });

            // Act
            var result = await step.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(StepActivityStages.ExecutionTerminated, result.WorkflowChain.First().StepActivities.Last().Activity);
        }
        
        [Fact]
        public async void Create_WithAsyncAndCancellation_Cancells()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            
            var step = Step<SimpleWorkflowState>.Create(async (_, terminate, ct) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
            });

            // Act
            var stepTask = step.ExecuteAsync(_workflowState, cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            // Assert
            Assert.ThrowsAsync<WorkflowActionException<SimpleWorkflowState>>(async () => await stepTask);
        }
        
        [Fact]
        public async void Create_WithSyncAndTerminate_Terminates()
        {
            // Arrange
            var step = Step<SimpleWorkflowState>.Create((_, terminate) => terminate());

            // Act
            var result = await step.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(StepActivityStages.ExecutionTerminated, result.WorkflowChain.First().StepActivities.Last().Activity);
        }
    }
}