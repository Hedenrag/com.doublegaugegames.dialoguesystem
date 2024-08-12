using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/DialogueSystem/CharacterDefinitionsAsset", fileName = "CharacterDefinitions")]
public class CharacterDefinitionsScriptableObject : ScriptableObject
{
    [SerializeField] CharacterDefinition[] characterDefinitions;

    public List<CharacterDefinition> CharacterDefinitions => characterDefinitions.ToList();

    Dictionary<string, CharacterDefinition> characterDefinitionsDict;

    private void OnEnable()
    {
        characterDefinitionsDict = new Dictionary<string, CharacterDefinition>();
        foreach(CharacterDefinition charDef in characterDefinitions)
        {
            characterDefinitionsDict.Add(charDef.GUID, charDef);
        }
#if UNITY_EDITOR
        lastIntLength = characterDefinitions.Length;
#endif
    }
#if UNITY_EDITOR
    int lastIntLength;
    const string defaultName = "Character";
    private void OnValidate()
    {
        if (characterDefinitions.Length > lastIntLength)
        {
            var character = characterDefinitions.Last();
            character.GetNewGUID();
            character.texture = null;
            int i = 0;
            string newName;
            do
            {
                newName = $"{defaultName} ({i})";
                i++;
            } while (characterDefinitions.Select(x => x.name).Contains(newName));
            character.name = newName;

            lastIntLength = characterDefinitions.Length;
        }
    }
#endif
}

[System.Serializable]
public class CharacterDefinition
{
    public Texture2D texture;
    public string name;

    [SerializeField] string guid = Guid.NewGuid().ToString();
    public string GUID => guid;
    public void GetNewGUID()
    {
        guid = Guid.NewGuid().ToString();
    }
}
