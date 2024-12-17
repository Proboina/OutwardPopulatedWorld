using System;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public class Choice
    {
        public string Text;
        public ConditionBase Condition;
        [XmlElement("NextNode")]
        public DialogueNodeBase NextNode;
    }
}
