using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SideLoader;
using System;
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
        private string TrainerPrefabPath = "editor/templates/TrainerTemplate";
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
                if (DialogueManager.HasDialogueForCharacer(item.Key))
                {
                    item.Value.OnSpawn += Character_OnSpawn;

                }          
            }

            GenerateDialogueSkillSynchroniser();
        }



        private void Character_OnSpawn(Character character, string _arg2)
        {
            try
            {
                Trainer existingTrainerComp = character.GetComponentInChildren<Trainer>();

                if (existingTrainerComp == null)
                {
                    GameObject trainerPrefab = Instantiate(Resources.Load<GameObject>(TrainerPrefabPath));
                    trainerPrefab.transform.SetParent(character.transform, false);
                    trainerPrefab.transform.position = character.transform.position;
                }

                DialogueTreeController graphController = character.gameObject.GetComponentInChildren<DialogueTreeController>();
                Graph graph = graphController.graph;

                DialogueTree DialogueTree = (DialogueTree)graph;


                DialogueActor ourActor = character.GetComponentInChildren<DialogueActor>();
                ourActor.SetName(character.Name);

                List<DialogueTree.ActorParameter> actors = (graph as DialogueTree).actorParameters;
                actors[0].actor = ourActor;
                actors[0].name = ourActor.name;
                DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(DialogueTree);

                DialogueManager.BuildDialogueForCharacter(character.UID, DialogueTree, dialogueTreeBuilder);
            }
            catch (Exception  e)
            {
                Debug.LogError($"On Character Spawn Exception \r\n {e}");
            }


        }
    }
}
