using System;
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
    public class ForLoopTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;

        public ForLoopTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
        }

        [Fact]
        public void StepMethods()
        {
            // Arrange
            var step1 = new BaseStep("one");
            var step2 = new BaseStep("two");
            var step3 = new BaseStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };

            var forLoop = new ForLoop<SimpleWorkflowState>(1);
            
            // ActSert a lot because who cares...
            forLoop.SetSteps(baseStepList);
            Assert.Equal(baseStepList, forLoop.Steps);
            
            forLoop.ClearSteps();
            Assert.Empty(forLoop.Steps);

            forLoop.AddStep(step1);
            Assert.Single(forLoop.Steps);
            Assert.Equal(step1, forLoop.Steps.First());
        }

        [Fact]
        public async void ForLoopCompletesExecution()
        {
            // Arrange
            var iterations = 2;
            
            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * baseStepList.Count, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        }
        
        [Fact]
        public async void DetermineIterationsTest()
        {
            // Arrange
            const int iterations = 2;
            
            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>((SimpleWorkflowState simpleWorkflowState) => iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * baseStepList.Count, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        }
        
        [Fact]
        public async void DetermineIterationsAsyncTest()
        {
            // Arrange
            const int iterations = 2;
            Task<int> DetermineIterations(SimpleWorkflowState simpleWorkflowState, CancellationToken cancellationToken) => Task.FromResult(iterations);

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(DetermineIterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations * baseStepList.Count, result.State.MyInteger);
            Assert.Equal(iterations *  baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration);
        }

        [Fact]
        public async void BreakForLoopTest()
        {
            // Arrange
            var iterations = 2;
            
            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var breakStep = new StopThatThrowsBreak("break-step");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, step2, breakStep };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);
            var nonBreakStepCount = baseStepList.Count(x => x.GetType() != breakStep.GetType());

            // Assert
            Assert.Equal(nonBreakStepCount, result.State.MyInteger);
            Assert.Equal(baseStepList.Count + 1, result.WorkflowChain.Count);
            Assert.Equal(0, forLoop.CurrentIteration);
        }

        [Fact]
        public async void ContinueForLoopTest()
        {
            // Arrange
            var iterations = 2;
            
            var step1 = new LoopStep("one");
            var continueStep = new StepThatThrowsContinue("break-step");
            var step3 = new LoopStep("three");

            var baseStepList = new List<IStep<SimpleWorkflowState>> { step1, continueStep, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.ExecuteAsync(_workflowState);

            // Assert
            Assert.Equal(iterations , result.State.MyInteger);
            Assert.Equal(iterations * 2 + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration); 
        }

        [Fact]
        public void Create_WithIterationCount_CreatesSuccessfully()
        {
            // Arrange
            var expectedIterationCount = 2;

            // Act
            var result = ForLoop<SimpleWorkflowState>.Create(expectedIterationCount);

            // Assert
            result.IterationCount.Should().Be(expectedIterationCount);
        }
        
        [Fact]
        public async void Create_WithIterationCountSync_CreatesSuccessfully()
        {
            // Arrange
            Func<SimpleWorkflowState, int> setIterationCountSync = _ => 2;
            var expectedIterationCount = setIterationCountSync(_workflowState.State);
            var forLoop = ForLoop<SimpleWorkflowState>.Create(setIterationCountSync);

            // Act
            await forLoop.ExecuteAsync(_workflowState);

            // Assert
            forLoop.IterationCount.Should().Be(expectedIterationCount);
        }
        
        [Fact]
        public async void Create_WithIterationCountAsync_CreatesSuccessfully()
        {
            // Arrange
            Func<SimpleWorkflowState?, CancellationToken, Task<int>> iterationCountAsync = (_, _) => Task.FromResult(2);
            var expectedIterationCount = await iterationCountAsync(_workflowState.State, default);
            var forLoop = ForLoop<SimpleWorkflowState>.Create(iterationCountAsync);

            // Act
            await forLoop.ExecuteAsync(_workflowState);

            // Assert
            forLoop.IterationCount.Should().Be(expectedIterationCount);
        }
    }
}