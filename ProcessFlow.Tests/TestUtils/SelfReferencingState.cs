using System.Collections.Generic;

namespace ProcessFlow.Tests.TestUtils
{
    public class SelfReferencingState
    {
        public List<ReferenceNode<int>> RelatedStates { get; set; }
    }

    public class ReferenceNode<T>
    {
        public ReferenceNode()
        {
            RelatedNodes = new List<ReferenceNode<T>>();
        }

        public T Value { get; set; }
        public List<ReferenceNode<T>> RelatedNodes { get; set; }
    }
}
