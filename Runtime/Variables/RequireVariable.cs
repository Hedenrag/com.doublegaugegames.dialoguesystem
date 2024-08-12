using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DGG.DialogueSystem
{


    [System.Serializable]
    public class RequireVariable
    {
        [SerializeField] public Rect position;
        [SerializeField] public List<DialogueInput> connectedDialogues;
        [SerializeField] public string variableName;
    }

    [System.Serializable]
    public struct DialogueInput
    {
        public string nodeGUID;
        public int index;

        public DialogueInput(string GUID, int index)
        {
            nodeGUID = GUID;
            this.index = index;
        }
    }

}