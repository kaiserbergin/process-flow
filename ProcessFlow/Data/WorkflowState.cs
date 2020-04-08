using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProcessFlow.Data
{
    public class WorkflowState<T>
    {
        public WorkflowState()
        {
            WorkflowChain = new LinkedList<WorkflowChainLink>();
        }

        public LinkedList<WorkflowChainLink> WorkflowChain;
        public T State { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
