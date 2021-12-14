using System.Collections.Generic;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public interface IStepSelector<TState>: IStep<TState> where TState : class
    {
        Dictionary<string, IStep<TState>> Options();
        IStepSelector<TState> SetOptions(Dictionary<string, IStep<TState>> options);
    }
}