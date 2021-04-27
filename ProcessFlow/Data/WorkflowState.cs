
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProcessFlow.Data
{
    public class WorkflowState<T> where T : class 
    {
        public WorkflowState()
        {
            WorkflowChain = new LinkedList<WorkflowChainLink>();
        }

        public LinkedList<WorkflowChainLink> WorkflowChain;
        public T State { get; set; }

        public override string ToString() =>
            JsonConvert.SerializeObject(this);
    }
}
