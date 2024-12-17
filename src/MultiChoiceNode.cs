using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace PopulatedWorld
{
    [Serializable]
    public class MultiChoiceNode : DialogueNodeBase
    {
        [XmlArray]
        [XmlArrayItem("Choice")]
        public List<Choice> Choices = new List<Choice>();

        public override DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode)
        {
            string[] choiceTexts = new string[Choices.Count];
            ConditionTask[] conditions = new ConditionTask[Choices.Count];

            for (int i = 0; i < Choices.Count; i++)
            {
                choiceTexts[i] = Choices[i].Text;
                conditions[i] = Choices[i].Condition?.CreateCondition();
            }

            var multiChoice = builder.AddMultipleChoiceNode(choiceTexts, conditions);

            if (parentNode != null)
            {
                tree.ConnectNodes(parentNode, multiChoice);
            }

            for (int i = 0; i < Choices.Count; i++)
            {
                if (Choices[i].NextNode != null)
                {
                    DialogueNodeBase currentNodeDef = Choices[i].NextNode;
                    DTNode firstNode = currentNodeDef.BuildNode(tree, builder, null);
                    DTNode currentNode = firstNode;

                    while (currentNodeDef.NextNode != null)
                    {
                        currentNodeDef = currentNodeDef.NextNode;
                        DTNode nextNode = currentNodeDef.BuildNode(tree, builder, null);
                        tree.ConnectNodes(currentNode, nextNode);
                        currentNode = nextNode;
                    }

                    builder.AddAnswerToMultipleChoice(multiChoice, i, choiceTexts[i], firstNode);
                }
            }

            return multiChoice;
        }
    }
}