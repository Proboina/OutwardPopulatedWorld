using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SideLoader;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PopulatedWorld
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "prob.populatedworld";
        public const string NAME = "Populated World";
        public const string VERSION = "1.0.0";
        internal static ManualLogSource Log;

        public static DialogueManager DialogueManager { get; private set; }


        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            DialogueManager = new DialogueManager(Paths.PluginPath);

            SL.OnPacksLoaded += SL_OnPacksLoaded;


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

            DialogueManager.BuildDialogueForCharacter(character.UID, DialogueTree, dialogueTreeBuilder);


            foreach (var item in DialogueTree._nodes)
            {
                Debug.Log(item);
            }

            //StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement("Hello this is the first thing I say.");



            ////yes I copy and pasted, if you're going to set up many like this I will copy over the proper structure you can use lol.

            //MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            //{
            //    "I have some bandages",
            //    "Whats up dog",
            //}, 
            //new ConditionTask[]
            //    {
            //        new HasCurrency()
            //        {
            //            AmountRequired = 100
            //        },
            //        null
            //});

            //InitialStatement.ConnectTo(DialogueTree, initialChoice);

            //string MountSimpleExplanation = $"I will give you 10 bandages for 100 silver";

            //dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation)).ConnectTo(DialogueTree, new RemoveMoneyAction(100)).ConnectTo(DialogueTree, new GiveItem(4400010));
            //dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Nothing dawg whasup with you", dialogueTreeBuilder.CreateNPCStatement("NOW IM SHOUTING"));
        }
    }
}
