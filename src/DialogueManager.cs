using NodeCanvas.DialogueTrees;
using SideLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace PopulatedWorld
{
    public class DialogueManager
    {
        #region Properties
        public Dictionary<string, DialogueDefinition> DialogueData { get; private set; }
        #endregion

        private readonly string RootFolder;
        private readonly string DialogueFolder;

        public DialogueManager(string rootFolder)
        {
            RootFolder = rootFolder;
            DialogueFolder = Path.Combine(RootFolder, "Dialogues");
            Debug.Log($"Initializing DialogueManager at: {RootFolder}");

            DialogueData = new Dictionary<string, DialogueDefinition>();
            LoadAllDialogueFiles();
        }

        private void LoadAllDialogueFiles()
        {
            Debug.Log("DialogueManager Loading Dialogue Definitions..");

            string[] directoriesInPluginsFolder = Directory.GetDirectories(RootFolder);
            foreach (var directory in directoriesInPluginsFolder)
            {
                string dialoguePath = Path.Combine(directory, "Dialogues");
                if (Directory.Exists(dialoguePath))
                {
                    string[] filePaths = Directory.GetFiles(dialoguePath, "*.xml", SearchOption.AllDirectories);
                    Debug.Log($"DialogueManager [{filePaths.Length}] Dialogue Definitions found.");

                    foreach (var filePath in filePaths)
                    {
                        DialogueDefinition dialogue = DeserializeFromXML<DialogueDefinition>(filePath);
    
                        if (dialogue != null)
                        {
                            Debug.Log($"Created Dialogue definition for character: {dialogue.CharacterUID}");
                            Debug.Log($"{dialogue}");
                            Debug.Log($"{dialogue.Nodes.Count}");
                            if (!DialogueData.ContainsKey(dialogue.CharacterUID))
                            {
                                DialogueData.Add(dialogue.CharacterUID, dialogue);
                            }
                        }
                    }
                }
            }
        }

        private T DeserializeFromXML<T>(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sideLoaderAssembly = typeof(SL_Effect).Assembly;

            var dialogueTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && (
                    typeof(DialogueNodeBase).IsAssignableFrom(t) ||
                    typeof(XMLActionNode).IsAssignableFrom(t) ||
                    typeof(ConditionBase).IsAssignableFrom(t) ||
                    typeof(Choice).IsAssignableFrom(t)));

            var effectTypes = sideLoaderAssembly.GetTypes()
                .Where(t => !t.IsAbstract && (
                    typeof(SL_Effect).IsAssignableFrom(t) ||
                    typeof(SL_EffectTransform).IsAssignableFrom(t)));

            var allTypes = dialogueTypes.Concat(effectTypes).ToArray();

            Debug.Log($"Found types: {string.Join(", ", allTypes.Select(t => t.Name))}");

            var serializer = new XmlSerializer(typeof(T), allTypes);
            using (StreamReader reader = new StreamReader(path))
            {
                try
                {
                    var result = (T)serializer.Deserialize(reader);
                    Debug.Log($"Deserialization completed: {result}");
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to deserialize: {ex}");
                    return default;
                }
            }
        }

        public DialogueDefinition GetDialogueForCharacter(string characterUID)
        {
            Plugin.Log.LogMessage($"Attempting to retrieve DialogueDefintion for {characterUID}.");
            if (DialogueData.TryGetValue(characterUID, out var dialogue))
            {

                Plugin.Log.LogMessage($"{dialogue}");
                return dialogue;
            }
            return null;
        }

        public bool HasDialogueForCharacer(string characterUID)
        {
            return DialogueData.ContainsKey(characterUID);
        }

        public void BuildDialogueForCharacter(string characterUID, DialogueTree tree, DialogueTreeBuilder builder)
        {
            var dialogue = GetDialogueForCharacter(characterUID);
            if (dialogue != null)
            {
                dialogue.Build(tree, builder);
            }
            else
            {
                Plugin.Log.LogError($"No dialogue definition found for character {characterUID}");
            }
        }
    }
}
