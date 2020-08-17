using System.Text.Json;

namespace ProcessFlow.Data
{
    public class StepSettings
    {
        public bool AutoProgress { get; set; }
        public bool TrackStateChanges { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
