
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProcessFlow.Data
{
    public class WorkflowState<T> where T : class 
    {
        public WorkflowState() { }

        public WorkflowState(StepSettings? stepSettings = null, T? state = null)
        {
            State = state;
            DefaultStepSettings = stepSettings;
        }

        public LinkedList<WorkflowChainLink> WorkflowChain = new LinkedList<WorkflowChainLink>();
        public StepSettings? DefaultStepSettings;
        public T? State { get; set; }

        public override string ToString() =>
            JsonConvert.SerializeObject(this);
    }
}
