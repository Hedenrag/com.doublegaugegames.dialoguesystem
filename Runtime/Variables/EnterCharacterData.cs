using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnterCharacterData : BaseDialogueNode
{
    [SerializeField] public string SpeakerCharacterGUID;
    [SerializeField] public bool EnterCharacter;
    [SerializeField] public bool CharacterLeft;
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


    public EnterCharacterData()
    {
        nextNodeGUID = new string[1];
    }
}
