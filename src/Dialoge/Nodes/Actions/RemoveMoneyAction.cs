using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using System;
using UnityEngine;

namespace PopulatedWorld
{
    public class RemoveMoneyAction : ActionNode
    {
        public int Amount;

        public RemoveMoneyAction()
        {
        }

        public RemoveMoneyAction(int amount)
        {
            Amount = amount;
        }

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking)
            {
                PlayerTalking.Inventory.RemoveMoneyAndUseGoldBar(Amount);
                return Status.Success;
            }

            return Status.Failure;
        }
    }


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
