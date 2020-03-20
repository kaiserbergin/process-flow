using System;

namespace ProcessFlow.Data
{
    public interface IClock
    {
        DateTimeOffset UtcNow();
    }

    [Serializable]
    public readonly struct Clock : IClock
    {
        public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
    }
}
