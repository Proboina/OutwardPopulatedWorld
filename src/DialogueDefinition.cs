using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public class DialogueDefinition
    {
        public string CharacterUID;
        public string InitialStatement;
        [XmlArray]
        [XmlArrayItem("Node")]
        public List<DialogueNodeBase> Nodes = new List<DialogueNodeBase>();

        public void Build(DialogueTree tree, DialogueTreeBuilder builder)
        {
            var initial = builder.SetInitialStatement(InitialStatement);
            Plugin.Log.LogMessage($"Node count {Nodes.Count}");

            if (Nodes.Count > 0)
            {
                Plugin.Log.LogMessage($"Connecting initial node to next");

                var nextNode = Nodes[0].BuildNode(tree, builder, null);
                Plugin.Log.LogMessage(nextNode);

                initial.ConnectTo(tree, nextNode);
            }
        }
    }
}
