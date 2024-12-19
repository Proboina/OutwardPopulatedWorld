﻿using NodeCanvas.Framework;

namespace PopulatedWorld
{
    public class HasItem : ConditionTask
    {
        public int ItemID;
        public int Amount;

        public HasItem()
        {
        }

        public HasItem(int itemID, int amount)
        {
            ItemID = itemID;
            Amount = amount;
        }

        public override bool OnCheck()
        {
            Character PlayerTalking = blackboard.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking)
            {
                return PlayerTalking.Inventory.OwnsItem(ItemID, Amount);
            }

            return false;
        }
    }
}