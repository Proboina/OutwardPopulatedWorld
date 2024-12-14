using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PopulatedWorld
{
    public static class DialogueManager
    {

        static string SLPack = "Populated World"; // SLpack of the mod
        static string testCharacterUID = "Proboina.Test"; //UID of the NPC we add dialogue to
        internal static SL_Character testCharacter; // just a variable
        internal static SL_Character Doofus;

        public static void Init()
        {
            var modPackName = SL.GetSLPack(SLPack);

            //Setup the npc so when spawned, dialogue is added
            testCharacter = modPackName.CharacterTemplates[testCharacterUID];
            testCharacter.OnSpawn += CierzoGossipSetup;
        }

         public static void CierzoGossipSetup(Character trainer, string _)
        {
            Dialogue_Setup(trainer,
                testCharacter,
                2, // number of dialogue/reply
                //intro dialogue
                "Yes? What do you need my friend? Interested in the latest gossip? Or are you here to learn some self defence?",
                //ask1
                "Can you show me your butt?",
                //reply1
                "Abso-fucking-lutely");
        }

        public static void Dialogue_Setup(Character trainer, SL_Character currentCharacter, int quantity,
           string introDialogue, string ask1, string reply1,
           string ask2 = "", string reply2 = "", string ask3 = "", string reply3 = "", string ask4 = "", string reply4 = "")
        {
            // remove unwanted components
            //GameObject.DestroyImmediate(trainer.GetComponent<CharacterStats>());
            //GameObject.DestroyImmediate(trainer.GetComponent<StartingEquipment>());

            trainer.gameObject.AddComponent<Trainer>();

            // set Dialogue Actor name
            trainer.gameObject.AddComponent<DialogueActor>();
            var actor = trainer.GetComponentInChildren<DialogueActor>();
            actor.SetName(currentCharacter.Name);

            // setup dialogue tree
            trainer.gameObject.AddComponent<DialogueTreeController>();
            var graphController = trainer.GetComponentInChildren<DialogueTreeController>();
            var graph = graphController.graph;

            // the template comes with an empty ActorParameter, we can use that for our NPC actor.
            var actors = (graph as DialogueTree)._actorParameters;
            actors[0].actor = actor;
            actors[0].name = actor.name;

            // setup the actual dialogue now
            var rootStatement = graph.AddNode<StatementNodeExt>();
            rootStatement.statement = new Statement(introDialogue);
            rootStatement.SetActorName(actor.name);

            var multiChoice1 = graph.AddNode<MultipleChoiceNodeExt>();
            if (quantity >= 2)
                multiChoice1.availableChoices.Add(new MultipleChoiceNodeExt.Choice { statement = new Statement { text = ask1 } });
            if (quantity >= 3)
                multiChoice1.availableChoices.Add(new MultipleChoiceNodeExt.Choice { statement = new Statement { text = ask2 } });
            if (quantity >= 4)
                multiChoice1.availableChoices.Add(new MultipleChoiceNodeExt.Choice { statement = new Statement { text = ask3 } });
            if (quantity >= 5)
                multiChoice1.availableChoices.Add(new MultipleChoiceNodeExt.Choice { statement = new Statement { text = ask4 } });


            StatementNodeExt answer2 = null;
            StatementNodeExt answer3 = null;
            StatementNodeExt answer4 = null;
            StatementNodeExt answer5 = null;

            // create some custom dialogue
            if (quantity >= 2)
            {
                answer2 = graph.AddNode<StatementNodeExt>();
                answer2.statement = new Statement(reply1);
                answer2.SetActorName(actor.name);
            }
            if (quantity >= 3)
            {
                answer3 = graph.AddNode<StatementNodeExt>();
                answer3.statement = new Statement(reply2);
                answer3.SetActorName(actor.name);
            }
            if (quantity >= 4)
            {
                answer4 = graph.AddNode<StatementNodeExt>();
                answer4.statement = new Statement(reply3);
                answer4.SetActorName(actor.name);
            }
            if (quantity >= 5)
            {
                answer5 = graph.AddNode<StatementNodeExt>();
                answer5.statement = new Statement(reply4);
                answer5.SetActorName(actor.name);
            }

            // ===== finalize nodes =====
            graph.allNodes.Clear();
            // add the nodes we want to use
            graph.allNodes.Add(rootStatement);
            graph.allNodes.Add(multiChoice1);
            if (quantity >= 2 && answer2 != null)
                graph.allNodes.Add(answer2);
            if (quantity >= 3 && answer3 != null)
                graph.allNodes.Add(answer3);
            if (quantity >= 4 && answer4 != null)
                graph.allNodes.Add(answer4);
            if (quantity >= 5 && answer5 != null)
                graph.allNodes.Add(answer5);
 
            graph.primeNode = rootStatement;

            //connect nodes
            graph.ConnectNodes(rootStatement, multiChoice1);
            if (quantity >= 2 && answer2 != null)
            {
                // prime node triggers the multiple choice
                graph.ConnectNodes(multiChoice1, answer2, 0);       // choice2: answer1
                graph.ConnectNodes(answer2, rootStatement); // - choice2 goes back to root node
            }
            if (quantity >= 3 && answer3 != null)
            {
                graph.ConnectNodes(multiChoice1, answer3, 1);       // choice3: answer2
                graph.ConnectNodes(answer3, rootStatement);         // - choice3 goes back to root node
            }
            if (quantity >= 4 && answer4 != null)
            {
                graph.ConnectNodes(multiChoice1, answer4, 2);       // choice3: answer2
                graph.ConnectNodes(answer4, rootStatement);    // choice1: open trainer
            }
            if (quantity >= 5 && answer5 != null)
            {
                graph.ConnectNodes(multiChoice1, answer5, 3);
                graph.ConnectNodes(answer5, rootStatement);
            }

            //graph.ConnectNodes(multiChoice1, openTrainer, quantity - 1);

            // set the trainer active
            trainer.gameObject.SetActive(true);
        }
    }
}
