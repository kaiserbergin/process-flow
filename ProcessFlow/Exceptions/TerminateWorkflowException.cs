using ProcessFlow.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessFlow.Exceptions
{
    public class TerminateWorkflowException : Exception
    {
    }

    public class TerminateWorkflowException<T> : Exception
    {
        public WorkflowState<T> WorkflowState;

        public TerminateWorkflowException(WorkflowState<T> workflowState)
        {
            WorkflowState = workflowState;
        }
    }
}
