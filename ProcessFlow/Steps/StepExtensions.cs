using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessFlow.Steps
{
    public static class StepExtensions
    {
        public static TStep SetNext<TStep, TState>(this Step<TState> source, TStep next)
             where TStep : Step<TState>
             where TState : class
        {
            source.SetNextStep(next);
            return next;
        }

        public static TStep SetPrevious<TStep, TState>(this Step<TState> source, TStep previous)
            where TStep : Step<TState>
            where TState : class
        {
            source.SetPreviousStep(previous);
            return previous;
        }
    }
}
