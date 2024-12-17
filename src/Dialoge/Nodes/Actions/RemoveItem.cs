using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using System;
using UnityEngine;

namespace PopulatedWorld
{
    public class RemoveItem : ActionNode
    {
        public int ItemID;
        public int Quantity;

        public RemoveItem()
        {

        }

        public RemoveItem(int itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking && PlayerTalking.Inventory.OwnsItem(ItemID, Quantity))
            {
                PlayerTalking.Inventory.RemoveItem(ItemID, Quantity);
            }

            return Status.Success;
        }
    }

    [Serializable]
    public class RemoveItemActionNode : XMLActionNode
    {
        public int ItemID;
        public int Quantity;

        public override DTNode CreateAction()
        {
            var action = new RemoveItem();
            action.ItemID = ItemID;
            action.Quantity = Quantity;
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
