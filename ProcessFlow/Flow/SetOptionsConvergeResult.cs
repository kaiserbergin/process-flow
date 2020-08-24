using System;
using System.Collections.Generic;

namespace ProcessFlow.Flow
{
    /// <summary>
    /// An object that is used as the return type for <see
    /// cref="StepExtensions.SetOptionsConverge{T}(SingleStepSelector{T}, List{Steps.Step{T}})"/>.
    /// The purpose of this type is to be able to set the next step for each step within the
    /// options list.
    /// </summary>
    /// <typeparam name="T">The type of the state object.</typeparam>
    public class SetOptionsConvergeResult<T>
        where T : class
    {
        private readonly List<Steps.Step<T>> _options;

        public SetOptionsConvergeResult(List<Steps.Step<T>> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Sets the next step for each of the steps in the options list and then returns the
        /// object passed into <paramref name="step"/>.
        /// </summary>
        /// <param name="step">The step to set as the next step for all steps in the options list.</param>
        /// <returns>The object passed into <paramref name="step"/>.</returns>
        public TStep SetNext<TStep>(TStep step)
            where TStep : Steps.Step<T>
        {
            foreach (var option in _options)
            {
                option.SetNext(step);
            }

            return step;
        }
    }
}