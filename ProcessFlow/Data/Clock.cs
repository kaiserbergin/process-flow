using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessFlow.Data
{
    public interface IClock
    {
        DateTimeOffset UtcNow();
    }

    public readonly struct Clock : IClock
    {
        public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
    }
}
