namespace ProcessFlow.Data
{
    public enum StepActivityStages
    {
        Executing,      
        Processing,
        ProcessCompleted,
        ProcessFailed,
        ExtensionProcessRunning,
        ExtensionProcessCompleted,
        ExtensionProcessFailed,
        StateExported,
        StateImporting,
        StateImportComplete,
        StateImportFailed,
        ExecutionComplete,
        ExecutionFailed
    }
}
