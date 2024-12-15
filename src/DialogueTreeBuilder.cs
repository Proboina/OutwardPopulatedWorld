using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

namespace PopulatedWorld
{

    public class DialogueTreeBuilder
    {
        public DialogueTree TargetDialogueTree { get; private set; }
        public DialogueTree.ActorParameter Actor { get; private set; }

        public DialogueTreeBuilder(DialogueTree targetDialogueTree, bool ClearGraph = true)
        {
            TargetDialogueTree = targetDialogueTree;
            Actor = TargetDialogueTree.actorParameters[0];
            if (ClearGraph) TargetDialogueTree.allNodes.Clear();
        }


        public StatementNodeExt SetInitialStatement(string InitialStatement)
        {
            // Add our root statement
            StatementNodeExt InitialStatementNode = CreateNPCStatement(InitialStatement);
            TargetDialogueTree.primeNode = InitialStatementNode;
            return InitialStatementNode;
        }


        public DTNode AddNode(DTNode NewNode)
        {
            if (!TargetDialogueTree.allNodes.Contains(NewNode))
            {
                TargetDialogueTree.allNodes.Add(NewNode);
                return NewNode;
            }

            return NewNode;
        }

        public MultipleChoiceNodeExt AddMultipleChoiceNode(string[] choices, ConditionTask[] Condition = null)
        {
            MultipleChoiceNodeExt multiChoice = TargetDialogueTree.AddNode<MultipleChoiceNodeExt>();

            for (int i = 0; i < choices.Length; i++)
            {
                MultipleChoiceNodeExt.Choice multipleChoice = new MultipleChoiceNodeExt.Choice()
                {
                    statement = new Statement()
                    {
                        text = choices[i]
                    },

                    condition = Condition != null && Condition[i] != null ? Condition[i] : null
                };

                multiChoice.availableChoices.Add(multipleChoice);
            }

            return multiChoice;
        }

        public StatementNodeExt CreateNPCStatement(string AnswerText, bool ContinueOnFinish = false)
        {
            StatementNodeExt AnswerStatement = TargetDialogueTree.AddNode<StatementNodeExt>();
            AnswerStatement.statement = new(AnswerText);
            AnswerStatement.SetActorName(Actor.name);
            AnswerStatement.ContinueOnStatementFinished = ContinueOnFinish;

            AddNode(AnswerStatement);

            return AnswerStatement;
        }

        public StatementNode CreatePlayerStatement(string AnswerText)
        {
            StatementNode AnswerStatement = TargetDialogueTree.AddNode<StatementNode>();
            AnswerStatement.statement = new(AnswerText);
            AddNode(AnswerStatement);
            return AnswerStatement;
        }

        public T AddAnswerToMultipleChoice<T>(MultipleChoiceNodeExt multiChoice, int answerIndex, T NewNode) where T : DTNode
        {
            TargetDialogueTree.ConnectNodes(multiChoice, NewNode, answerIndex);
            return NewNode;
        }


        public ConditionNode AddConditionalAnswerToMultiChoice(MultipleChoiceNodeExt multiChoice, DialogueTree DT, int answerIndex, ConditionTask Condition)
        {
            ConditionNode ConditionNode = AddAnswerToMultipleChoice(multiChoice, answerIndex, DT.AddNode<ConditionNode>());
            ConditionNode.condition = Condition;
            return ConditionNode;
        }


        public T AddAnswerToMultipleChoice<T>(MultipleChoiceNodeExt multiChoice, int answerIndex, string AnswerText, T AnswerNode = null) where T : DTNode
        {
            StatementNodeExt AnswerStatement = CreateNPCStatement(AnswerText);

            if (AnswerNode != null)
            {
                AddNode(AnswerNode);
                TargetDialogueTree.ConnectNodes(multiChoice, AnswerStatement, answerIndex);
                TargetDialogueTree.ConnectNodes(AnswerStatement, AnswerNode);
                return AnswerNode;
            }

            TargetDialogueTree.ConnectNodes(multiChoice, AnswerStatement, answerIndex);
            return (T)(AnswerStatement as DTNode);
        }
    }
}
