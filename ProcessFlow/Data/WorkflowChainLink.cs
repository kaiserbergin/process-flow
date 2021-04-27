using ProcessFlow.Extensions;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProcessFlow.Data
{
    public class WorkflowChainLink
    {
        public string StepName { get; set; }
        public string StepIdentifier { get; set; }
        public int SequenceNumber { get; set; }
        public List<StepActivity> StepActivities { get; set; } = new List<StepActivity>();
        private byte[] StateSnapshot;

        public void SetStateSnapshot(object obj)
        {
            if (obj != null)
                StateSnapshot = obj.Zippify();
        }

        public T GetUncompressedStateSnapshot<T>() where T : class
        {
            if (StateSnapshot == null)
                return null;
            return StateSnapshot.Unzippify<T>();
        }

        public byte[] GetCompressedStateSnapshot() => StateSnapshot;

        public override bool Equals(object obj)
        {
            return obj is WorkflowChainLink link &&
                   StepName == link.StepName &&
                   StepIdentifier == link.StepIdentifier &&
                   SequenceNumber == link.SequenceNumber &&
                   EqualityComparer<List<StepActivity>>.Default.Equals(StepActivities, link.StepActivities) &&
                   EqualityComparer<byte[]>.Default.Equals(StateSnapshot, link.StateSnapshot);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StepName, StepIdentifier, SequenceNumber, StepActivities, StateSnapshot);
        }

        public override string ToString() =>
            JsonConvert.SerializeObject(this);

        public static bool operator ==(WorkflowChainLink left, WorkflowChainLink right)
        {
            return EqualityComparer<WorkflowChainLink>.Default.Equals(left, right);
        }

        public static bool operator !=(WorkflowChainLink left, WorkflowChainLink right)
        {
            return !(left == right);
        }
    }
}