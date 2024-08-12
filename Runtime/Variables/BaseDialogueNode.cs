using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class BaseDialogueNode
{
    [SerializeField] internal string GUID = string.Empty;

    [SerializeField] public string[] nextNodeGUID;

    public virtual string NextNodeGUID(int option = 0)
    {
        if (nextNodeGUID.Length == 0) return null;
        return nextNodeGUID[0];
    }

    public BaseDialogueNode() 
    { 
        GUID = System.Guid.NewGuid().ToString();
    }

#if UNITY_EDITOR

    public Rect position;

#endif

}
