using System;
using System.Text.Json;

namespace ProcessFlow.Data
{
    public class StepSettings
    {
        public bool AutoProgress { get; set; }

        public override bool Equals(object obj)
        {
            return obj is StepSettings settings &&
                   AutoProgress == settings.AutoProgress;
        }

        public override int GetHashCode()
        {
            return -712961855 + AutoProgress.GetHashCode();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
