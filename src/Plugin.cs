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
            ourActor.SetName("NAME");

            List<DialogueTree.ActorParameter> actors = (graph as DialogueTree).actorParameters;
            actors[0].actor = ourActor;
            actors[0].name = ourActor.name;

            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(DialogueTree);
            StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement("HWODHOWDHWOD");


    
            //yes I copy and pasted, if you're going to set up many like this I will copy over the proper structure you can use lol.

            MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
                {
                    "Can you look after my current mount?",
                    "I want to retrieve a mount.",
                    "Can you teach me a little about mounts?",
                    "I want to buy some color berries",
                    "Do you have any creatures for sale?",
                    "Do you have any unique creatures for sale?"
                }, null);

            InitialStatement.ConnectTo(DialogueTree, initialChoice);

            string MountSimpleExplanation = $"You can buy, find and I have even heard in rare cases <i> create </i> - a mount." +
            $"There are usually two ways to get a new mount, find a whistle and use it or find an egg use it and 12 hours later it will hatch. " +
            $"Most of my colleagues in the Stablery business sell some kind of mount to get you moving. Ask them!";

            string MountSimpleExplanation2 = $"<b>Most</b> mounts will require feeding! " +
                $"They all have a favourite food in the case of carnivores it is usually <b>Jewel Bird Meat</b> and <b>Herbivores seem to love Marsh Mellons</b> favourite foods fill a mount up better than other foods" +
                $"Most mounts will have a maximum carry weight which they will move slower than usual the closer they are to reaching it.";

            string MountSimpleExplanation3 = $"Then there are eggs you can find Pearl Bird nests out in the world sometimes these may contain eggs that you are able to raise into mounts sometimes the plumage on the Pearl birds varies!" +
                $"I hear you can also find eggs for Manticores and Tuanosaurs sometimes from their corpses! I hear even Alphas!";

            string MountSimpleExplanation4 = $"And there are also disturbing rumours about a <b>Gold Lich</b> experimenting with living pearl bird egg using <b>Living Gold</b>, can you imagine? How does such a creature even exist?" +
                $"Reminds me of that <b>Mad Pirate Cpt</b> famed for his unusual Pearl Bird companion he had some how transmuted <b>Living Obsidian</b> and a Pearl bird egg to create an incredibly fast mount that did not require feeding but is such an advantage worth the cost?";

            string MountSimpleExplanation5 = $"Anyway .. Here are two skills you can use to Summon and Dismiss your <b>Active</b> mount - you will still have to visit a stable master in order to change it though, we don't charge for this service.";

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, "Aye, I will take care of them.", dialogueTreeBuilder.CreateNPCStatement("SAY SOMETHING"));
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Here's a list of what you have in my stables.", dialogueTreeBuilder.CreateNPCStatement("SAY SOMETHING 2"));
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 2, MountSimpleExplanation, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation2))
                .ConnectTo(DialogueTree, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation3))
                .ConnectTo(DialogueTree, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation4))
                .ConnectTo(DialogueTree, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation5));

        }
    }
}
