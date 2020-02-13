using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProcessFlow.Flow
{
    public class Step<T> : IStep<T>
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        private Step<T> _next;
        private Step<T> _previous;

        private StepSettings _processorSettings;

        private IProcessor<T> _processor;

        public Step(string name = null, string id = null, StepSettings stepSettings = null, IProcessor<T> processor = null)
        {
            Name = name ?? GetType().Name.ToString();
            Id = id ?? Guid.NewGuid().ToString("N");
            _processorSettings = stepSettings ?? new StepSettings();
            _processor = processor;
        }

        public Step()
        {
            Name = GetType().Name.ToString();
            Id = Guid.NewGuid().ToString("N");
            _processorSettings = new StepSettings();
        }

        public Step<T> SetProcessor(IProcessor<T> processor)
        {
            _processor = processor;
            return this;
        }

        public Step<T> Next()
        {
            return _next;
        }

        public Step<T> Previous()
        {
            return _previous;
        }

        public async Task<WorkflowState<T>> Process(WorkflowState<T> workflowState)
        {
            try
            {
                if (_processor != null)
                {
                    workflowState.State = await _processor.Process(workflowState.State);
                }

                UpdateWorkflowChain(workflowState);

                if (typeof(Step<T>) != this.GetType())
                    await ExecuteExtensionProcess(workflowState);

                if (_processorSettings.AutoProgress)
                    return await ProcessNext(workflowState);
            }
            catch (Exception exception)
            {
                throw new WorkflowActionException<T>(exception.Message, exception.InnerException, workflowState);
            }

            return workflowState;
        }

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

        private async Task<WorkflowState<T>> ProcessNext(WorkflowState<T> workflowState)
        {
            if (_next != null)
                return await _next.Process(workflowState);
            else
                return workflowState;
        }

        public Step<T> SetNext(Step<T> processor)
        {
            _next = processor;
            return _next;
        }

        public Step<T> SetPrevious(Step<T> processor)
        {
            _previous = processor;
            return _previous;
        }

        private void OnEnter()
        {

        }

        private void OnExit()
        {

        }

        private void OnProcessStarted()
        {

        }

        private void OnProcessCompleted()
        {

        }

        private void OnProcessFailed()
        {

        }

        private void OnExtensionProcessStarted()
        {

        }

        private void OnExtensionProcessCompleted()
        {

        }

        private void OnExtensionProcessFailed()
        {

        }
    }
}
