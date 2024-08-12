using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DGG.DialogueSystem
{

    public class DialogueNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        DialogueEditorWindow _window;
        DialogueEditorGraph _graphView;

        private Texture2D _indentationIcon;

        public void Configure(DialogueEditorWindow window, DialogueEditorGraph graphView)
        {
            _window = window;
            _graphView = graphView;

            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                //new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    level = 1, userData = new DialogueNode()
                },
                new SearchTreeEntry(new GUIContent("Set Variable Node", _indentationIcon))
                {
                    level = 1, userData = new SetVariableNode()
                },
                new SearchTreeEntry(new GUIContent("Requirement Node", _indentationIcon))
                {
                    level = 1, userData = new RequirementNode()
                },
                new SearchTreeEntry(new GUIContent("Character Enter Node", _indentationIcon))
                {
                    level = 1, userData = new EnterCharacterInSceneNode()
                },
                new SearchTreeEntry(new GUIContent("Switch Node", _indentationIcon))
                {
                    level = 1, userData = new DialogueSwitch()
                },
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case DialogueNode _:
                    _graphView.AddDialogueNode(graphMousePosition);
                    return true;
                case SetVariableNode _:
                    _graphView.AddVariableNode(graphMousePosition);
                    return true;
                case RequirementNode _:
                    _graphView.AddRequirementNode(graphMousePosition);
                    return true;
                case EnterCharacterInSceneNode _:
                    _graphView.AddCharacterEnterNode(graphMousePosition);
                    return true;
                case DialogueSwitch _:
                    _graphView.AddSwitchNode(graphMousePosition);
                    return true;
            }
            return false;
        }
    }

}