using ProcessFlow.Extensions;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProcessFlow.Data
{
    public sealed class WorkflowChainLink
    {
        private byte[]? _stateSnapshot;

        internal WorkflowChainLink(
            string stepName,
            string stepIdentifier,
            int sequenceNumber,
            StepActivity stepActivity
        )
        {
            StepName = stepName;
            StepIdentifier = stepIdentifier;
            SequenceNumber = sequenceNumber;
            StepActivities.Add(stepActivity);
        }

        public string StepName { get; }
        public string StepIdentifier { get; }
        public int SequenceNumber { get; }
        public List<StepActivity> StepActivities { get; set; } = new List<StepActivity>();

        public void SetStateSnapshot(object? obj)
        {
            if (obj != null)
                _stateSnapshot = obj.Zippify();
        }

        public T? GetUncompressedStateSnapshot<T>() where T : class =>
            _stateSnapshot?.Unzippify<T>();

        public byte[]? GetCompressedStateSnapshot() => _stateSnapshot;

        public override bool Equals(object obj)
        {
            return obj is WorkflowChainLink link &&
                   StepName == link.StepName &&
                   StepIdentifier == link.StepIdentifier &&
                   SequenceNumber == link.SequenceNumber &&
                   EqualityComparer<List<StepActivity>>.Default.Equals(StepActivities, link.StepActivities) &&
                   (
                       _stateSnapshot == null && link._stateSnapshot == null ||
                       _stateSnapshot != null && link._stateSnapshot != null &&
                       EqualityComparer<byte[]>.Default.Equals(_stateSnapshot, link._stateSnapshot)
                   );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StepName, StepIdentifier, SequenceNumber, StepActivities, _stateSnapshot);
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