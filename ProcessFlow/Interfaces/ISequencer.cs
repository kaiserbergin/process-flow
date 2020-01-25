using ProcessFlow.Flow;
using System.Collections.Generic;

namespace ProcessFlow.Interfaces
{
    public interface ISequencer<T>
    {
        Sequencer<T> AddStep(Step<T> processor);
        List<Step<T>> GetSequence();
        Sequencer<T> SetSequence(List<Step<T>> sequence);
    }
}