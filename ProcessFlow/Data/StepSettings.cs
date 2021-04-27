using Newtonsoft.Json;

namespace ProcessFlow.Data
{
    public class StepSettings
    {
        public bool AutoProgress { get; set; }
        public bool TrackStateChanges { get; set; }
        public override string ToString() =>
            JsonConvert.SerializeObject(this);
    }
}
