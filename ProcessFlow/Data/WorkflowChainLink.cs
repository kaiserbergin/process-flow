using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProcessFlow.Data
{
    public class WorkflowChainLink
    {
        public string StepName { get; set; }
        public string StepIdentifier { get; set; }
        public int SequenceNumber { get; set; }
        public List<StepActivity> StepActivities { get; set; } = new List<StepActivity>();
        public string StateSnapshot { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WorkflowChainLink link &&
                   StepName == link.StepName &&
                   StepIdentifier == link.StepIdentifier &&
                   SequenceNumber == link.SequenceNumber &&
                   EqualityComparer<List<StepActivity>>.Default.Equals(StepActivities, link.StepActivities) &&
                   StateSnapshot == link.StateSnapshot;
        }

        public override int GetHashCode()
        {
            var hashCode = -480268711;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StepName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StepIdentifier);
            hashCode = hashCode * -1521134295 + SequenceNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<StepActivity>>.Default.GetHashCode(StepActivities);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StateSnapshot);
            return hashCode;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

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
