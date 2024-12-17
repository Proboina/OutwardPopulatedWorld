using NodeCanvas.DialogueTrees;
using System;

namespace PopulatedWorld
{
    [Serializable]
    public class RemoveMoneyActionNode : XMLActionNode
    {
        public int Amount;

        public override DTNode CreateAction()
        {
            var action = new RemoveMoneyAction();
            action.Amount = Amount;
            return action;
        }

        public override DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode)
        {
            var node = base.BuildNode(tree, builder, parentNode);

            builder.AddNode(node);
            if (parentNode != null)
            {
                parentNode.ConnectTo(tree, node);
            }
            return node;
        }
    }

}