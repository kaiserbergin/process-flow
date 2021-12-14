using System.Collections.Generic;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public interface IStepDictionarySelector<TState>: IStep<TState> where TState : class
    {
        Dictionary<string, IStep<TState>> Options();
        IStepDictionarySelector<TState> SetOptions(Dictionary<string, IStep<TState>> options);
    }
}