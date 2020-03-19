using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProcessFlow.Steps
{
    public abstract class Step<T>
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        private Step<T> _next;
        private Step<T> _previous;

        private StepSettings _stepSettings;

        public Step(string name = null, StepSettings stepSettings = null)
        {
            Name = name ?? GetType().Name;
            Id = Guid.NewGuid().ToString("N");
            _stepSettings = stepSettings;
        }

        public Step<T> Next()
        {
            return _next;
        }

        public Step<T> Previous()
        {
            return _previous;
        }

        public async Task<WorkflowState<T>> Execute(WorkflowState<T> workflowState)
        {
            try
            {
                workflowState.State = await Process(workflowState.State);
                UpdateWorkflowChain(workflowState);

                await ExecuteExtensionProcess(workflowState);

                if (_stepSettings != null && _stepSettings.AutoProgress)
                    return await ExecuteNext(workflowState);
            }
            catch (TerminateWorkflowException)
            {
                UpdateTerminatedWorkflowChain(workflowState);
            }
            catch (Exception exception)
            {
                throw new WorkflowActionException<T>(exception.Message, exception.InnerException, workflowState);
            }

            return workflowState;
        }

        public void Terminate() => throw new TerminateWorkflowException();
        
        protected abstract Task<T> Process(T state);

        protected virtual Task<WorkflowState<T>> ExecuteExtensionProcess(WorkflowState<T> workflowState) => Task.FromResult(workflowState);

        private void UpdateWorkflowChain(WorkflowState<T> workflowState)
        {
            var chain = workflowState.WorkflowChain;
            var previousLink = chain?.Last;

            var link = new WorkflowChainLink()
            {
                StepName = Name,
                StepIdentifier = Id,
                SequenceNumber = previousLink?.Value?.SequenceNumber + 1 ?? 0,
                StateSnapshot = JsonSerializer.Serialize(workflowState.State) 
            };

            chain.AddLast(link);
        }

        private void UpdateTerminatedWorkflowChain(WorkflowState<T> workflowState)
        {
            UpdateWorkflowChain(workflowState);
        }


        protected async Task<WorkflowState<T>> ExecuteNext(WorkflowState<T> workflowState)
        {
            if (_next != null)
                return await _next.Execute(workflowState);
            else
                return workflowState;
        }

        public Step<T> SetNext(Step<T> step)
        {
            _next = step;
            return _next;
        }

        public Step<T> SetPrevious(Step<T> step)
        {
            _previous = step;
            return _previous;
        }
    }
}
