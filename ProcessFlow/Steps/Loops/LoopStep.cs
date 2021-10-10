using ProcessFlow.Data;
using ProcessFlow.Exceptions;

namespace ProcessFlow.Steps.Loops
{
    public abstract class LoopStep<T> : Step<T> where T : class
    {
        private int _currentIteration = 0;
        public int CurrentIteration => _currentIteration;
        
        public LoopStep(string? name = null, StepSettings? stepSettings = null, IClock? clock = null)
            : base(name, stepSettings, clock) { }
        
        protected void Break() => throw new BreakException();
        protected void Continue() => throw new ContinueException();
        internal void SetIteration(int currentIteration) => _currentIteration = currentIteration;
    }
}