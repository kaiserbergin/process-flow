using ProcessFlow.Data;
using ProcessFlow.Flow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessFlow.Interfaces
{
    public interface ISingleStepSelector<T>
    {
        Task<Step<T>> Select(List<Step<T>> options, WorkflowState<T> workflowState);
    }
}
