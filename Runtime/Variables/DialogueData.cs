using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("DialogueSystemEditor")]

namespace DGG.DialogueSystem
{
[System.Serializable]
    public class DialogueData : BaseDialogueNode
    {
        [SerializeField] public string text;

        [SerializeField] public List<DialogueOption> dialogueOption;

        public CharacterDefinitionsScriptableObject CharacterDefinition
        {
            get
            {
#if UNITY_EDITOR
                if (characterDefinition == null)
                {
                    characterDefinition = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterDefinitionsScriptableObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(UnityEditor.AssetDatabase.FindAssets("t:" + typeof(CharacterDefinitionsScriptableObject).Name).First()));
                }
#endif
                return characterDefinition;
            }
            set
            {
                characterDefinition = value;
            }
        }
        [SerializeField] private CharacterDefinitionsScriptableObject characterDefinition;

        [SerializeField] public string characterGUID;
        /// <summary>
        /// false = left, true = right
        /// </summary>
        [SerializeField] public bool characterSide = false;

        public override string NextNodeGUID(int option = 0)
        {
            return dialogueOption[option].NextNodeGUID;
        }

        public DialogueData()
        {
            text = "Some text";
            dialogueOption = new List<DialogueOption>();
        }

        [System.Serializable]
        public class DialogueOption
        {
            [SerializeReference] DialogueData dataParent;
            [SerializeField] public string[] requirements;
            [SerializeField] public string answer;
            [SerializeField] string nextNodeGUID;
#if UNITY_EDITOR
            [SerializeField] public string optionName;
#endif
            public string NextNodeGUID { get { return nextNodeGUID; } set { nextNodeGUID = value; dataParent.UpdateOutGUIDS(); } }

            public DialogueOption(DialogueData parent)
            {
                dataParent = parent;
                requirements = new string[0];
                answer = "Continue";
                nextNodeGUID = null;
            }
        }

        void UpdateOutGUIDS()
        {
            nextNodeGUID = new string[dialogueOption.Count];
            for (int i = 0; i < dialogueOption.Count; i++)
            {
                nextNodeGUID[i] = dialogueOption[i].NextNodeGUID;
            }
        }
    }

}