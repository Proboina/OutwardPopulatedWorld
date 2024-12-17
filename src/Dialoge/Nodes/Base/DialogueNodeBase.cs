using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public abstract class DialogueNodeBase
    {
        [XmlElement("NextNode")]
        public DialogueNodeBase NextNode;

        public abstract DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode);
    }

}
