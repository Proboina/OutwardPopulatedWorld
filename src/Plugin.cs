using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PopulatedWorld
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "prob.populatedworld";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Populated World";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            SL.OnPacksLoaded += SL_OnPacksLoaded;

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        public string SLPack = "Populated World";
        //    static string testCharacterUID = "Proboina.Test";
        private void SL_OnPacksLoaded()
        {
            var pack = SL.GetSLPack(SLPack);

            if (pack != null)
            {
                SL_Character character = pack.CharacterTemplates["Proboina.Test"];

                character.OnSpawn += Character_OnSpawn;
            }
        }

        private string TrainerPrefabPath = "editor/templates/TrainerTemplate";

        private void Character_OnSpawn(Character character, string _arg2)
        {
            Log.LogMessage(character);

            GameObject trainerPrefab = Instantiate(Resources.Load<GameObject>(TrainerPrefabPath));
            trainerPrefab.transform.SetParent(character.transform, false);
            Log.LogMessage(trainerPrefab);


            DialogueTreeController graphController = character.gameObject.GetComponentInChildren<DialogueTreeController>();
            Graph graph = graphController.graph;
            trainerPrefab.transform.position = character.transform.position;
            DialogueTree DialogueTree = (DialogueTree)graph;
            DialogueActor ourActor = character.GetComponentInChildren<DialogueActor>();
            ourActor.SetName(character.Name);

            List<DialogueTree.ActorParameter> actors = (graph as DialogueTree).actorParameters;
            actors[0].actor = ourActor;
            actors[0].name = ourActor.name;

            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(DialogueTree);
            StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement("Hello this is the first thing I say.");


           
            //yes I copy and pasted, if you're going to set up many like this I will copy over the proper structure you can use lol.

            MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "I have some bandages",
                "Whats up dog",
            }, 
            new ConditionTask[]
                {
                    new HasCurrency()
                    {
                        AmountRequired = 100
                    },
                    null
            });

            InitialStatement.ConnectTo(DialogueTree, initialChoice);

            string MountSimpleExplanation = $"I will give you 10 bandages for 100 silver";

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation)).ConnectTo(DialogueTree, new RemoveMoneyAction(100)).ConnectTo(DialogueTree, new GiveItem(4400010)));
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Nothing dawg whasup with you", dialogueTreeBuilder.CreateNPCStatement("NOW IM SHOUTING"));
        }
    }
    public class HasCurrency : ConditionTask
    {
        public int AmountRequired;

        public HasCurrency()
        {
        }

        public HasCurrency(int amountRequired)
        {
            AmountRequired = amountRequired;
        }

        public override bool OnCheck()
        {
            Character PlayerTalking = blackboard.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking && PlayerTalking.Inventory.AvailableMoney >= AmountRequired)
            {
                return true;
            }

            return false;
        }
    }
    public class GiveItem : ActionNode
    {
        public int ItemID;

        public GiveItem()
        {

        }

        public GiveItem(int itemID)
        {
            ItemID = itemID;
        }

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

            if (PlayerTalking)
            {
                PlayerTalking.Inventory.ReceiveItemReward(ItemID, 1, false);
            }

            return Status.Success;
        }
    }
}
