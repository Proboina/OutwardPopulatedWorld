﻿using NodeCanvas.Framework;
using System;

namespace PopulatedWorld
{
    public class HasCurrency : ConditionTask
    {
        public int AmountRequired;

        public HasCurrency()
        {
        }

        public HasCurrency(int amountRequired)
        {
            AmountRequired = amountRequired;
        }

        public override bool OnCheck()
        {
            Character PlayerTalking = blackboard.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking && PlayerTalking.Inventory.AvailableMoney >= AmountRequired)
            {
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class CurrencyCondition : ConditionBase
    {
        public int Amount;

        public override ConditionTask CreateCondition()
        {
            return new HasCurrency() { AmountRequired = Amount };
        }
    }
}
