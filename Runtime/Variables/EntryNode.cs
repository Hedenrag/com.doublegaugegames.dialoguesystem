using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntryNode : BaseDialogueNode
{
    public EntryNode()
    {
        nextNodeGUID = new string[1];
    }
}
