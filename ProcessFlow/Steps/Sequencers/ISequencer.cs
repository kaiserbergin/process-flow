using System.Collections.Generic;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Sequencers
{
    public interface ISequencer<TState> : IStep<TState> where TState : class
    {
        ISequencer<TState> AddStep(IStep<TState> processor);
        List<IStep<TState>> GetSequence();
        ISequencer<TState> SetSequence(List<IStep<TState>> sequence);
    }
}