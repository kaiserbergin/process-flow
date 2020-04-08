using System;

namespace ProcessFlow.Data
{
    public readonly struct StepActivity
    {
        public StepActivity(StepActivityStages activity, DateTimeOffset? dateTimeOffset = null, IClock clock = null)
        {
            Activity = activity;
            clock = clock ?? new Clock();
            DateTimeOffset = dateTimeOffset.HasValue ? dateTimeOffset.Value : clock.UtcNow();
        }

        public DateTimeOffset DateTimeOffset { get; }
        public StepActivityStages Activity { get; }
    }
}
