using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public abstract class XMLActionNode : DialogueNodeBase
    {
        public abstract DTNode CreateAction();

        public override DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode)
        {
            var action = CreateAction();
            builder.AddNode(action);


            if (parentNode != null)
            {
                parentNode.ConnectTo(tree, action);
            }

            if (NextNode != null)
            {
                var nextNode = NextNode.BuildNode(tree, builder, action);
                return nextNode; 
            }

            return action;
        }
    }
}
