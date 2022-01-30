using System.Collections.Generic;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Forks
{
    public interface IFork<T> : IStep<T> where T : class
    {
        Fork<T> AddStep(IStep<T> processor);
        List<IStep<T>> GetSteps();
        Fork<T> SetSteps(List<IStep<T>> sequence);
    }
}