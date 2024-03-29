using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ProcessFlow.Data;
using ProcessFlow.Steps.Base;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps.Loops
{
    public class WhileLoopTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public WhileLoopTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState> { State = new SimpleWorkflowState() };
        }

        [Fact]
        public void StepMethods()
        {
            // Arrange
            var step1 = new BaseStep("one");
            var step2 = new BaseStep("two");
            var step3 = new BaseStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };

            var whileLoop = new WhileLoop<SimpleWorkflowState>(shouldContinue: state => false);
            
            // ActSert a lot because who cares...
            whileLoop.SetSteps(baseStepList);
            Assert.Equal(baseStepList, whileLoop.Steps);
            
            whileLoop.ClearSteps();
            Assert.Empty(whileLoop.Steps);

            whileLoop.AddStep(step1);
            Assert.Single(whileLoop.Steps);
            Assert.Equal(step1, whileLoop.Steps.First());
        }

        [Fact]
        public async void Create_WithShouldContinueSync_Succeeds()
        {
            // Arrange
            var step1 = new BaseStep("one");
            var step2 = new BaseStep("two");
            var step3 = new BaseStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            // Act
            var whileLoop = WhileLoop<SimpleWorkflowState>.Create(_ => false, steps: baseStepList);
            var result = await whileLoop.ExecuteAsync(_workflowState);
            
            // Assert
            whileLoop.Steps.Count.Should().Be(baseStepList.Count);
            result.State.MyInteger.Should().Be(0);
        }
        
        [Fact]
        public async void Create_WithShouldContinueAsync_Succeeds()
        {
            // Arrange
            var step1 = new BaseStep("one");
            var step2 = new BaseStep("two");
            var step3 = new BaseStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            // Act
            var whileLoop = WhileLoop<SimpleWorkflowState>.Create(async (_, _) => await Task.FromResult(false), steps: baseStepList);
            var result = await whileLoop.ExecuteAsync(_workflowState);
            
            // Assert
            whileLoop.Steps.Count.Should().Be(baseStepList.Count);
            result.State.MyInteger.Should().Be(0);
        }
        
        
        [Fact]
        public async void ShouldContinueTest()
        {
            // Arrange
            const int iterations = 2;

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            bool ShouldContinue(SimpleWorkflowState simpleWorkflowState) => simpleWorkflowState.MyInteger < iterations * baseStepList.Count;
            
            var forLoop = new WhileLoop<SimpleWorkflowState>(ShouldContinue, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * baseStepList.Count, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        }
        
        [Fact]
        public async void ShouldContinueAsyncTest()
        {
            // Arrange
            const int iterations = 2;

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            Task<bool> ShouldContinueAsync(SimpleWorkflowState simpleWorkflowState, CancellationToken cancellationToken) => 
                Task.FromResult(simpleWorkflowState.MyInteger < iterations * baseStepList.Count);
            
            var forLoop = new WhileLoop<SimpleWorkflowState>(ShouldContinueAsync, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * baseStepList.Count, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        }

        [Fact]
        public async void BreakWhileLoopTest()
        {
            // Arrange
            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var breakStep = new StopThatThrowsBreak("break-step");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, breakStep };
            
            var forLoop = new WhileLoop<SimpleWorkflowState>(shouldContinue: state => true, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);
            var nonBreakStepCount = baseStepList.Count(x => x.GetType() != breakStep.GetType());

            // Assert
            Assert.Equal(nonBreakStepCount, result.State.MyInteger);
            Assert.Equal(baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(0, forLoop.CurrentIteration);
        }

        [Fact]
        public async void ShouldContinueWithCaughtContinueTest()
        {
            // Arrange
            const int iterations = 2;

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var controlStop = new StepThatThrowsContinue("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, controlStop };
            var nonControlStepCount = baseStepList.Count(x => x.GetType() != controlStop.GetType());
            
            bool ShouldContinue(SimpleWorkflowState simpleWorkflowState) => simpleWorkflowState.MyInteger < iterations * nonControlStepCount;
            
            var forLoop = new WhileLoop<SimpleWorkflowState>(ShouldContinue, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * nonControlStepCount, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        } 
    }
}