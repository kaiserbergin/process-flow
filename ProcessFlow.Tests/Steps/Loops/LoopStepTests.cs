using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps.Loops
{
    public class LoopStepTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public LoopStepTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState> { State = new SimpleWorkflowState() };
        }
        
        [Fact]
        public async void Create_WithTerminate_ThrowsTerminate()
        {
            // Arrange
            var loopStep = LoopStep<SimpleWorkflowState>.Create((_, terminate, _, _, _, _) =>
            {
                terminate();
                return Task.CompletedTask;
            });

            // Act
            var result = await loopStep.ExecuteAsync(_workflowState);

            // Assert
            result.State.MyInteger.Should().Be(0);
            result.WorkflowChain.First.Value.StepActivities.Count.Should().Be(2);
            result.WorkflowChain.First.Value.StepActivities.ToList().Last().Activity.Should().Be(StepActivityStages.ExecutionTerminated);
        }
        
        [Fact]
        public async void Create_WithBreak_ThrowsBreakException()
        {
            // Arrange
            var loopStep = LoopStep<SimpleWorkflowState>.Create((_, _, @break, _, _, _) =>
            {
                @break();
                return Task.CompletedTask;
            });

            Func<Task> executeLoopStep = () => loopStep.ExecuteAsync(_workflowState);

            // Actssert
            await executeLoopStep.Should().ThrowAsync<BreakException>();
        }
        
        [Fact]
        public async void Create_WithContinue_ThrowsContinueException()
        {
            // Arrange
            var loopStep = LoopStep<SimpleWorkflowState>.Create((_, _, _, @continue, _, _) =>
            {
                @continue();
                return Task.CompletedTask;
            });

            Func<Task> executeLoopStep = () => loopStep.ExecuteAsync(_workflowState);

            // Actssert
            await executeLoopStep.Should().ThrowAsync<ContinueException>();
        }
    }
}