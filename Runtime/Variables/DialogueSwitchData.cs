using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSwitchData : BaseDialogueNode
{
    [SerializeField] internal string defaultNextNodeGUID;
    [SerializeField] internal List<NextNode> nextNodes = new List<NextNode>();

    public override string NextNodeGUID(int option = 0)
    {
        for (int i = 0; i < nextNodes.Count; i++)
        {
            foreach(var req in nextNodes[i].requirements)
            {
                char firstLetter = req[0];
                string result;
                bool inverted = firstLetter == '!';
                if (inverted) { result = req[1..]; } else { result = req; }
                bool declared = DialogueHolder.declaredVariables.Contains(result);
                if (!(inverted ^ declared))
                {
                    continue;
                }
                return nextNodes[i].nextNodeGUID;
            }
        }
        return defaultNextNodeGUID;
    }
}

[System.Serializable]
internal class NextNode
{
    public string[] requirements;
    public string nextNodeGUID;

    internal NextNode(string[] requirements, string nextNodeGUID)
    {
        this.requirements = requirements;
        this.nextNodeGUID = nextNodeGUID;
    }
}