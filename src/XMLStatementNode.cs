using NodeCanvas.DialogueTrees;
using System;
using System.Collections.Generic;

namespace PopulatedWorld
{
    [Serializable]
    public class XMLStatementNode : DialogueNodeBase
    {
        public string Text;
        public bool AutoMoveToNext = false;

        public override DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode)
        {
            UnityEngine.Debug.Log($"Building XMLStatementNode with text: {Text}");
            var statement = builder.CreateNPCStatement(Text);
            statement.ContinueOnStatementFinished = AutoMoveToNext;

            if (parentNode != null)
            {
                UnityEngine.Debug.Log("Connecting to parent node");
                tree.ConnectNodes(parentNode, statement); 
            }

            if (NextNode != null)
            {
                UnityEngine.Debug.Log($"Building next node of type: {NextNode.GetType().Name}");
                var nextNode = NextNode.BuildNode(tree, builder, statement);
                tree.ConnectNodes(statement, nextNode);
            }

            return statement;
        }
    }
}
