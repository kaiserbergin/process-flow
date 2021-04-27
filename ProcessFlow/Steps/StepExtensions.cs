using ProcessFlow.Data;
using System.Collections.Generic;

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

        public static Fork<TState> Fork<TState>(this Step<TState> source) where TState : class
        {
            var fork = new Fork<TState>();
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this Step<TState> source, string name = null, StepSettings stepSettings = null) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings);
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this Step<TState> source, List<Step<TState>> steps, string name = null, StepSettings stepSettings = null) where TState : class
        {
            var fork = new Fork<TState>(steps, name, stepSettings);
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this Step<TState> source, string name = null, StepSettings stepSettings = null, params Step<TState>[] steps) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings, steps);
            source.SetNextStep(fork);
            return fork;
        }
    }
}
