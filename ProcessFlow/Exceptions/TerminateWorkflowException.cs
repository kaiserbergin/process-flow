﻿using ProcessFlow.Data;
using System;

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
