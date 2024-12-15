using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
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
}
