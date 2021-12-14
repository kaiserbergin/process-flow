using System.Collections.Generic;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Selectors
{
    public interface IStepListSelector<TState> : IStep<TState> where TState : class
    {
        List<IStep<TState>> Options();
        AbstractStepListSelector<TState> SetOptions(List<IStep<TState>> options);
    }
}