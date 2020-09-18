using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Steps;
using ProcessFlow.Steps.Loops;
using ProcessFlow.Tests.TestUtils;
using Xunit;

namespace ProcessFlow.Tests.Steps.Loops
{
    public class ForLoopTests
    {
        private WorkflowState<SimpleWorkflowState> _workflowState;
        private SimpleWorkflowState _originalWorfklowState;

        public ForLoopTests()
        {
            _workflowState = new WorkflowState<SimpleWorkflowState>() { State = new SimpleWorkflowState() };
            _originalWorfklowState = _workflowState.State.DeepCopy();
        }

        [Fact]
        public void StepMethods()
        {
            // Arrange
            var step1 = new BaseStep("one");
            var step2 = new BaseStep("two");
            var step3 = new BaseStep("three");

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, step2, step3 }; 
            
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

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.Execute(_workflowState);

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
            int DetermineIterations(SimpleWorkflowState simpleWorkflowState) => iterations;

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(DetermineIterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.Execute(_workflowState);

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
            Task<int> DetermineIterations(SimpleWorkflowState simpleWorkflowState) => Task.FromResult(iterations);

            var step1 = new LoopStep("one");
            var step2 = new LoopStep("two");
            var step3 = new LoopStep("three");

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, step2, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(DetermineIterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.Execute(_workflowState);

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

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, step2, breakStep };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.Execute(_workflowState);
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

            var baseStepList = new List<Step<SimpleWorkflowState>> { step1, continueStep, step3 };
            
            var forLoop = new ForLoop<SimpleWorkflowState>(iterations, name: "foo", steps: baseStepList);
            
            // Act
            var result = await forLoop.Execute(_workflowState);

            // Assert
            Assert.Equal(iterations , result.State.MyInteger);
            Assert.Equal(iterations * 2 + 1, result.WorkflowChain.Count);
            Assert.Equal(iterations, forLoop.CurrentIteration); 
        }
    }
}