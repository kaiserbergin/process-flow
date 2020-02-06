using ProcessFlow.Data;
using System;
using System.Runtime.Serialization;

namespace ProcessFlow.Exceptions
{
    public class WorkflowActionException<T> : Exception
    {
        public WorkflowState<T> WorkflowState;

        public WorkflowActionException(WorkflowState<T> workflowState)
        {
            WorkflowState = workflowState;
        }

        public WorkflowActionException(string message, WorkflowState<T> workflowState) : base(message) 
        {
            WorkflowState = workflowState;
        }

        public WorkflowActionException(string message, Exception innerException, WorkflowState<T> workflowState) : base(message, innerException)
        {
            WorkflowState = workflowState;
        }

        protected WorkflowActionException(SerializationInfo info, StreamingContext context, WorkflowState<T> workflowState) : base(info, context)
        {
            WorkflowState = workflowState;
        }
    }
}
