using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

namespace PopulatedWorld
{
    public static class NodeExtensions
    {
        public static DTNode ConnectTo(this DTNode sourceNode, DialogueTree DT, DTNode TargetNode, int sourceIndex = -1, int targetIndex = -1)
        {
            if (!DT.allNodes.Contains(TargetNode))
            {
                DT.allNodes.Add(TargetNode);
            }

            DT.ConnectNodes(sourceNode, TargetNode, sourceIndex, targetIndex);
            return TargetNode;
        }

        public static ConditionNode SetCondition(this ConditionNode sourceNode, ConditionTask Condition)
        {
            if (Condition != null) sourceNode.condition = Condition;
            return sourceNode;
        }

        public static DTNode OnSuccess(this ConditionNode sourceNode, DialogueTree DT, DTNode SuccessfulNode)
        {
            if (!DT.allNodes.Contains(SuccessfulNode))
            {
                DT.allNodes.Add(SuccessfulNode);
            }

            DT.ConnectNodes(sourceNode, SuccessfulNode, 0, -1);
            return SuccessfulNode;
        }

        public static DTNode OnFailure(this ConditionNode sourceNode, DialogueTree DT, DTNode FailureNode)
        {
            if (!DT.allNodes.Contains(FailureNode))
            {
                DT.allNodes.Add(FailureNode);
            }

            DT.ConnectNodes(sourceNode, FailureNode, 1, -1);
            return FailureNode;
        }
    }
}
