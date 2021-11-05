using ProcessFlow.Data;
using System.Collections.Generic;

namespace ProcessFlow.Steps
{
    public static class StepExtensions
    {
        public static TStep SetNext<TStep, TState>(this AbstractStep<TState> source, TStep next)
             where TStep : AbstractStep<TState>
             where TState : class
        {
            source.SetNextStep(next);
            return next;
        }

        public static TStep SetPrevious<TStep, TState>(this AbstractStep<TState> source, TStep previous)
            where TStep : AbstractStep<TState>
            where TState : class
        {
            source.SetPreviousStep(previous);
            return previous;
        }

        public static Fork<TState> Fork<TState>(this AbstractStep<TState> source, string? name = null, StepSettings? stepSettings = null) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings);
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this AbstractStep<TState> source, List<AbstractStep<TState>> steps, string? name = null, StepSettings? stepSettings = null) where TState : class
        {
            var fork = new Fork<TState>(steps, name, stepSettings);
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this AbstractStep<TState> source, string? name = null, StepSettings? stepSettings = null, params AbstractStep<TState>[] steps) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings, steps);
            source.SetNextStep(fork);
            return fork;
        }
    }
}
