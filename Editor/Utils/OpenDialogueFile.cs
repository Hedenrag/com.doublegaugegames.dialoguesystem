using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEngine;

namespace DGG.DialogueSystem
{

    public class OpenDialogueFile
    {
        [OnOpenAsset]
        //Handles opening the editor window when double-clicking project files
        public static bool OnOpenAsset(int instanceID, int line)
        {
            DialogueHolder project = EditorUtility.InstanceIDToObject(instanceID) as DialogueHolder;
            if (project != null)
            {
                DialogueEditorWindow.OpenProjectFile(project);
                return true;
            }
            return false;
        }
    }

}