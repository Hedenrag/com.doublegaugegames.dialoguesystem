using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DGG.DialogueSystem
{


    [CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue", order = 0)]
    public class DialogueHolder : ScriptableObject
    {
        [SerializeField] internal EntryNode entryNode;
        [SerializeField] internal List<DeclareVariable> variables = new List<DeclareVariable>();
        [SerializeField] internal List<EnterCharacterData> characters = new List<EnterCharacterData>();
        [SerializeField] internal List<DialogueData> dialogues = new List<DialogueData>();
        [SerializeField] internal List<DialogueSwitchData> switches = new List<DialogueSwitchData>();

        [SerializeField] internal List<RequireVariable> requiredVariables = new();

        internal static List<string> declaredVariables;

        Dictionary<string, BaseDialogueNode> dialogueFinder;

        BaseDialogueNode currentNode = null;

        public void UpdateDictionary()
        {
            dialogueFinder = new Dictionary<string, BaseDialogueNode>();
            foreach (var decVar in variables) dialogueFinder.Add(decVar.GUID, decVar);
            foreach (var charVar in characters) dialogueFinder.Add(charVar.GUID, charVar);
            foreach (var dialVar in dialogues) dialogueFinder.Add(dialVar.GUID, dialVar);
        }

        /// <summary>
        /// Returns the next dialogue node or null if the dialogue has ended.
        /// </summary>
        /// <param name="option">The index of the selected response (starting at 0)</param>
        /// <returns></returns>
        public DialogueData GetNextDialogue(int option)
        {
            if (currentNode == null) { currentNode = entryNode; }

            currentNode = dialogueFinder[currentNode.NextNodeGUID(option)];

            while (currentNode != null && currentNode is not DialogueData)
            {
                currentNode = dialogueFinder[currentNode.NextNodeGUID(0)];
            }
            return (DialogueData)currentNode;
        }

        private void OnEnable()
        {
            UpdateDictionary();
        }

        public void DeclareVariable(string var)
        {
            if (!declaredVariables.Contains(var))
                declaredVariables.Add(var);
        }
        public void RemoveVariable(string var)
        {
            declaredVariables.Remove(var);
        }
    }

}