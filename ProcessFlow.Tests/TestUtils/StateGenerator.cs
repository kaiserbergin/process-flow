using System.Collections.Generic;

namespace ProcessFlow.Tests.TestUtils
{
    public static class StateGenerator
    {
        public static SelfReferencingState GenerateSelfRefereningState()
        {
            var state = new SelfReferencingState();

            var firstNode = new ReferenceNode<int> { Value = 0 };
            var secondNode = new ReferenceNode<int> { Value = 1 };
            var thirdNode = new ReferenceNode<int> { Value = 2 };
            var fourthNode = new ReferenceNode<int> { Value = 3 };

            firstNode.RelatedNodes.Add(secondNode);
            secondNode.RelatedNodes.Add(firstNode);
            secondNode.RelatedNodes.Add(thirdNode);
            thirdNode.RelatedNodes.Add(secondNode);
            thirdNode.RelatedNodes.Add(firstNode);
            firstNode.RelatedNodes.Add(fourthNode);

            state.RelatedStates = new List<ReferenceNode<int>> { firstNode, secondNode, thirdNode, fourthNode };

            return state;
        }
    }
}
