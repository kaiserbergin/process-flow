using System.Threading;
using System.Threading.Tasks;
using ProcessFlow.Data;

namespace ProcessFlow.Steps
{
    public interface IStep<TState> where TState : class
    {
        string Name { get; }
        string Id { get; }
        StepSettings? StepSettings { get; }
        AbstractStep<TState>? Next();
        AbstractStep<TState>? Previous();
        Task<WorkflowState<TState>> ExecuteAsync(WorkflowState<TState> workflowState, CancellationToken cancellationToken = default);
        void Terminate();
        AbstractStep<TState> SetNextStep(AbstractStep<TState> step);
        AbstractStep<TState> SetPreviousStep(AbstractStep<TState> step);
    }
}