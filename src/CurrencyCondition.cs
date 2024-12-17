using NodeCanvas.Framework;
using System;

namespace PopulatedWorld
{
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
