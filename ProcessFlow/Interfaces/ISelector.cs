using ProcessFlow.Flow;
using ProcessFlow.Interfaces;
using System.Collections.Generic;

namespace ProcessFlow.Interfaces

{
    public interface ISelector<T>
    {
        List<Step<T>> Options();
        Selector<T> SetOptions(List<Step<T>> options);
        Selector<T> SetStepSelector(ISingleStepSelector<T> stepSelector);
    }
}