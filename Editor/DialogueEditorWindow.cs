using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace DGG.DialogueSystem
{

public class DialogueEditorWindow : EditorWindow
{
    static bool creatingWindow = false;

    public DialogueEditorGraph graphView;

    public static DialogueEditorWindow CreateGraphWindow()
    {
        var window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
        return window;
    }

    private void OnEnable()
    {
        ConstructGraphView();
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueEditorGraph(this)
        {
            name = "Dialogue Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    public void ResetWindow()
    {
        foreach (var edge in graphView.edges)
        {
            edge.input.Disconnect(edge);
            graphView.RemoveElement(edge);
        }
        foreach (var node in graphView.nodes)
        {
            graphView.RemoveElement(node);
        }
    }
    public void ClearWindow()
    {
        foreach(var node in graphView.nodes)
        {
            graphView.RemoveElement(node);
        }
        foreach(var edge in graphView.edges)
        {
            graphView.RemoveElement(edge);
        }
    }
    public void CloseWindow()
    {
        Close();
    }

    public static void OpenProjectFile(DialogueHolder project)
    {
        creatingWindow = true;
        var window = CreateGraphWindow();
        window.graphView.LoadFile(project);
        creatingWindow = false;
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    //private void OnBecameInvisible()
    //{
    //    graphView.SaveFile();
    //}

    private void OnLostFocus()
    {
        graphView.SaveFile();
    }

    private void OnFocus()
    {
        if (!creatingWindow && !graphView.CheckDialogueExistance())
        {
            Close();
        }
    }
}

}