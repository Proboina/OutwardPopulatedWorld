using NodeCanvas.Framework;
using System;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public abstract class ConditionBase
    {
        public abstract ConditionTask CreateCondition();
    }
}
