using ProcessFlow.Data;
using ProcessFlow.Exceptions;
using ProcessFlow.Steps.Base;

namespace ProcessFlow.Steps.Loops
{
    public abstract class AbstractLoopStep<TState> : AbstractStep<TState> where TState : class
    {
        private int _currentIteration = 0;
        public int CurrentIteration => _currentIteration;

        public AbstractLoopStep(string? name = null, StepSettings? stepSettings = null, IClock? clock = null)
            : base(name, stepSettings, clock)
        {
        }

        public void Break() => throw new BreakException();

        public void Continue() => throw new ContinueException();


        internal void SetIteration(int currentIteration) => _currentIteration = currentIteration;
    }
}