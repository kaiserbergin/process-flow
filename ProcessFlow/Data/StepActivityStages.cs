using System;

namespace ProcessFlow.Data
{
    [Serializable]
    public enum StepActivityStages
    {
        Executing,
        ExecutionCompleted,
        ExecutionFailed,
        ExecutionTerminated,
        StateExported,
        StateImporting,
        StateImportCompleted,
        StateImportFailed
    }
}
