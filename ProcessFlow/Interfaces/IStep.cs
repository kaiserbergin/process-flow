using ProcessFlow.Data;
using ProcessFlow.Flow;
using System.Threading.Tasks;

namespace ProcessFlow.Interfaces
{
    public interface IStep<T>
    {
        string Id { get; }
        string Name { get; }

        Step<T> Next();
        Step<T> Previous();
        Task<WorkflowState<T>> Process(WorkflowState<T> workflowState);
        Step<T> SetNext(Step<T> processor);
        Step<T> SetPrevious(Step<T> processor);
        Step<T> SetProcessor(IProcessor<T> processor);
    }
}