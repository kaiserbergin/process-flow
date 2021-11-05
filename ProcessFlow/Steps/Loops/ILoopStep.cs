namespace ProcessFlow.Steps.Loops
{
    public interface ILoopStep<TStep> : IStep<TStep> where TStep : class
    {
        int CurrentIteration { get; }
        void Break();
        void Continue();
    }
}