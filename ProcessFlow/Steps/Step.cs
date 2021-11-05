using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public abstract class Step<TState> where TState : class
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        private Step<TState>? _next;
        private Step<TState>? _previous;

        private StepSettings? _stepSettings;
        private IClock _clock;

        public Step(string? name = null, StepSettings? stepSettings = null, IClock? clock = null)
        {
            Name = name ?? GetType().Name;
            Id = Guid.NewGuid().ToString("N");
            _stepSettings = stepSettings;
            _clock = clock ?? new Clock();
        }

        public Step<TState>? Next()
        {
            return _next;
        }

        public Step<TState>? Previous()
        {
            return _previous;
        }

        public async Task<WorkflowState<TState>> ExecuteAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken = default)
        {
            var currentLink = CreateWorkflowChainLink(workflowState);

            try
            {
                workflowState.State = await ProcessAsync(workflowState.State, cancellationToken);

                if (_stepSettings?.TrackStateChanges ?? workflowState.DefaultStepSettings?.TrackStateChanges ?? false) TakeDataSnapShot(workflowState, currentLink);

                await ExecuteExtensionProcessAsync(workflowState, cancellationToken);
                
                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionCompleted, currentLink);
            }
            catch (LoopJumpException)
            {
                throw;
            }
            catch (TerminateWorkflowException)
            {
                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionTerminated, currentLink);
            }
            catch (Exception exception)
            {
                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionFailed, currentLink);
                throw new WorkflowActionException<TState>("Exception in Process Flow execution. See InnerException for details.", exception, workflowState);
            }

            if (_stepSettings?.AutoProgress ?? workflowState.DefaultStepSettings?.AutoProgress ?? false)
                return await ExecuteNextAsync(workflowState, cancellationToken);

            return workflowState;
        }

        [DoesNotReturn]
        public void Terminate() => throw new TerminateWorkflowException();

        protected abstract Task<TState?> ProcessAsync(TState? state, CancellationToken cancellationToken);

        protected virtual Task<WorkflowState<TState>> ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken) => Task.FromResult(workflowState);

        private WorkflowChainLink CreateWorkflowChainLink(WorkflowState<TState> workflowState)
        {
            var chain = workflowState.WorkflowChain;
            var previousLink = chain.Last;

            var link = new WorkflowChainLink(
                stepName: Name,
                stepIdentifier: Id,
                sequenceNumber: previousLink?.Value?.SequenceNumber + 1 ?? 0,
                stepActivity: new StepActivity(StepActivityStages.Executing, _clock.UtcNow())
            );
            
            chain.AddLast(link);

            return link;
        }

        private void TakeDataSnapShot(WorkflowState<TState> workflowState, WorkflowChainLink link) =>
            link.SetStateSnapshot(workflowState.State);

        private void AddActivityToWorkflowChainLink(StepActivityStages stepActivityStage, WorkflowChainLink link) =>
            link.StepActivities.Add(new StepActivity(stepActivityStage, _clock.UtcNow()));


        protected async Task<WorkflowState<TState>> ExecuteNextAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            if (_next != null)
                return await _next.ExecuteAsync(workflowState, cancellationToken);
            else
                return workflowState;
        }

        public Step<TState> SetNextStep(Step<TState> step)
        {
            _next = step;
            return _next;
        }

        public Step<TState> SetPreviousStep(Step<TState> step)
        {
            _previous = step;
            return _previous;
        }
    }
}