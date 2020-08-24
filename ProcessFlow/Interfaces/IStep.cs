using ProcessFlow.Data;
using ProcessFlow.Flow;
using System.Threading.Tasks;

namespace ProcessFlow.Interfaces
{
    public interface IStep<T> where T : class
    {
        string Id { get; }
        string Name { get; }

        Step<T> Next();
        Step<T> Previous();
        Task<WorkflowState<T>> Process(WorkflowState<T> workflowState);

        TStep SetNext<TStep>(TStep processor)
            where TStep : Step<T>;

        TStep SetPrevious<TStep>(TStep processor)
            where TStep : Step<T>;

        Step<T> SetProcessor(IProcessor<T> processor);
    }
}