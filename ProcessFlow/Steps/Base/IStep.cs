using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps.Base
{
    public interface IStep<TState> where TState : class
    {
        string Name { get; }
        string Id { get; }
        StepSettings? StepSettings { get; }
        IStep<TState>? Next();
        IStep<TState>? Previous();
        Task<WorkflowState<TState>> ExecuteAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken = default);
        void Terminate();
        IStep<TState> SetNextStep(IStep<TState> step);
        IStep<TState> SetPreviousStep(IStep<TState> step);
    }
}