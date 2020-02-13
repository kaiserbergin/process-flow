namespace ProcessFlow.Data
{
    public enum StepActivityStages
    {
        Entered,
        Exited,
        Processing,
        Processed,
        ProcessFailed,
        ExtensionProcessRunning,
        ExtensionProcessCompleted,
        ExtensionProcessFailed,
        StateRequested,
        StateRefreshed,
        Failed
    }
}
