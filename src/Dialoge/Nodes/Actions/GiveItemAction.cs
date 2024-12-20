using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SideLoader;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace PopulatedWorld
{
    public class GiveItemAction : ActionNode
    {
        public int ItemID;
        public int Quantity = 1;

        public GiveItemAction()
        {

        }

        public GiveItemAction(int itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking)
            {
                PlayerTalking.Inventory.ReceiveItemReward(ItemID, Quantity, false);
            }


            return Status.Success;
        }
    }

    [Serializable]
    public class GiveItemActionNode : XMLActionNode
    {
        public int ItemID;
        public int Quantity;

        public override DTNode CreateAction()
        {
            var action = new GiveItemAction();
            action.ItemID = ItemID;
            action.Quantity = Quantity;
            return action;
        }


    }

}
