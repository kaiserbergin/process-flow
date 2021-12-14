using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;
using ProcessFlow.Exceptions;

namespace ProcessFlow.Steps.Base
{
    public abstract class AbstractStep<TState> : IStep<TState> where TState : class
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public StepSettings? StepSettings { get; private set; }

        private IStep<TState>? _next;
        private IStep<TState>? _previous;

        private IClock _clock;

        public AbstractStep(string? name = null, StepSettings? stepSettings = null, IClock? clock = null)
        {
            Name = name ?? GetType().Name;
            Id = Guid.NewGuid().ToString("N");
            StepSettings = stepSettings;
            _clock = clock ?? new Clock();
        }

        public IStep<TState>? Next()
        {
            return _next;
        }

        public IStep<TState>? Previous()
        {
            return _previous;
        }

        public async Task<WorkflowState<TState>> ExecuteAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken = default)
        {
            var currentLink = CreateWorkflowChainLink(workflowState);

            try
            {
                await ProcessAsync(workflowState.State, cancellationToken);

                TakeDataSnapShot(workflowState, currentLink);

                await ExecuteExtensionProcessAsync(workflowState, cancellationToken);

                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionCompleted, currentLink);
            }
            catch (LoopJumpException)
            {
                throw;
            }
            catch (TerminateWorkflowException)
            {
                TakeDataSnapShot(workflowState, currentLink);
                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionTerminated, currentLink);
                return workflowState;
            }
            catch (Exception exception)
            {
                TakeDataSnapShot(workflowState, currentLink);
                AddActivityToWorkflowChainLink(StepActivityStages.ExecutionFailed, currentLink);
                throw new WorkflowActionException<TState>("Exception in Process Flow execution. See InnerException for details.", exception, workflowState);
            }

            if (StepSettings?.AutoProgress ?? workflowState.DefaultStepSettings?.AutoProgress ?? false)
                return await ExecuteNextAsync(workflowState, cancellationToken);

            return workflowState;
        }

        [DoesNotReturn]
        public void Terminate() => throw new TerminateWorkflowException();

        protected abstract Task ProcessAsync(TState? state, CancellationToken cancellationToken);

        protected virtual Task ExecuteExtensionProcessAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken) => Task.FromResult(workflowState);

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

        private void TakeDataSnapShot(WorkflowState<TState> workflowState, WorkflowChainLink link)
        {
            if (StepSettings?.TrackStateChanges ?? workflowState.DefaultStepSettings?.TrackStateChanges ?? false)
                link.SetStateSnapshot(workflowState.State);
        }

        private void AddActivityToWorkflowChainLink(StepActivityStages stepActivityStage, WorkflowChainLink link) =>
            link.StepActivities.Add(new StepActivity(stepActivityStage, _clock.UtcNow()));


        protected async Task<WorkflowState<TState>> ExecuteNextAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken)
        {
            if (_next != null)
                return await _next.ExecuteAsync(workflowState, cancellationToken);
            return workflowState;
        }

        public IStep<TState> SetNextStep(IStep<TState> step)
        {
            _next = step;
            return _next;
        }

        public IStep<TState> SetPreviousStep(IStep<TState> step)
        {
            _previous = step;
            return _previous;
        }
    }
}