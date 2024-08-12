using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DGG.DialogueSystem
{


    [System.Serializable]
    public class DeclareVariable : BaseDialogueNode
    {
        [SerializeField] public string VariableName;
        [SerializeField] public bool Set;

        public DeclareVariable()
        {
            nextNodeGUID = new string[1];
        }
    }

}