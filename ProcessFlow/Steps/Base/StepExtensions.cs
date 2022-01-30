using System.Collections.Generic;
using ProcessFlow.Data;
using ProcessFlow.Steps.Forks;

namespace ProcessFlow.Steps.Base
{
    public static class StepExtensions
    {
        public static TStep SetNext<TStep, TState>(this IStep<TState> source, TStep next)
             where TStep : IStep<TState>
             where TState : class
        {
            source.SetNextStep(next);
            return next;
        }

        public static TStep SetPrevious<TStep, TState>(this IStep<TState> source, TStep previous)
            where TStep : IStep<TState>
            where TState : class
        {
            source.SetPreviousStep(previous);
            return previous;
        }

        public static Fork<TState> Fork<TState>(this IStep<TState> source, string? name = null, StepSettings? stepSettings = null, List<IStep<TState>>? steps = null) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings, steps);
            source.SetNextStep(fork);
            return fork;
        }

        public static Fork<TState> Fork<TState>(this IStep<TState> source, string? name = null, StepSettings? stepSettings = null, params IStep<TState>[] steps) where TState : class
        {
            var fork = new Fork<TState>(name, stepSettings, steps);
            source.SetNextStep(fork);
            return fork;
        }
    }
}
