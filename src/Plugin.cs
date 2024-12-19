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
using static MapMagic.ObjectPool;

namespace PopulatedWorld
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "prob.populatedworld";
        public const string NAME = "Populated World";
        public const string VERSION = "1.0.0";
        public string SLPack = "Populated World";
        internal static ManualLogSource Log;

        public static DialogueManager DialogueManager { get; private set; }
        public static DialogueEffectContainer DialogueEffectContainer { get; private set; }

        public static Item Skill { get; set; }

        public static Plugin Instance { get; private set; }

        internal void Awake()
        {
            Instance = this;
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            DialogueManager = new DialogueManager(Paths.PluginPath);
            DialogueEffectContainer = this.gameObject.AddComponent<DialogueEffectContainer>();
            SL.OnPacksLoaded += SL_OnPacksLoaded;


            new Harmony(GUID).PatchAll();
        }


        public void GenerateDialogueSkillSynchroniser()
        {
            if (Skill == null)
            {
                Skill = ItemManager.Instance.GenerateItemNetwork(4100170);

                Debug.Log($"Skill generated ? {Skill}");

                if (Skill)
                {
                    DontDestroyOnLoad(Skill.gameObject);
                }

            }
        }


        private void SL_OnPacksLoaded()
        {

            foreach (var item in CustomCharacters.Templates)
            {
                item.Value.OnSpawn += Character_OnSpawn;
            }

            GenerateDialogueSkillSynchroniser();
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
        }
    }
}
