using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

namespace DGG.DialogueSystem
{

    public class DialogueEditorGraph : GraphView
    {

        private DialogueNodeSearchWindow _searchWindow;
        private DialogueEditorWindow _window;

        public EnterGraphNode entryNode;

        DialogueHolder currentFile;

        public DialogueEditorGraph(DialogueEditorWindow window)
        {
            _window = window;
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow(window);
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node && port.direction != startPortView.direction && port.portType == startPortView.portType)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void AddSearchWindow(DialogueEditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<DialogueNodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public void AddDialogueNode(Vector2 position)
        {
            AddElement(new DialogueNode(position));
        }
        public void AddVariableNode(Vector2 position)
        {
            AddElement(new SetVariableNode(position));
        }
        public void AddRequirementNode(Vector2 position)
        {
            AddElement(new RequirementNode(position));
        }
        public void AddCharacterEnterNode(Vector2 position)
        {
            AddElement(new EnterCharacterInSceneNode(position));
        }
        public void AddSwitchNode(Vector2 position)
        {
            AddElement(new DialogueSwitch(position));
        }

        public void LoadFile(DialogueHolder project)
        {
            if (CheckDialogueExistance())
                SaveFile();
            _window.ClearWindow();
            currentFile = project;
            AddElement(new EnterGraphNode(project.entryNode));
            Dictionary<string, BaseNode> nodeDict = new();
            foreach (var declaration in project.variables)
            {
                var setVar = new SetVariableNode(declaration);
                AddElement(setVar);
                nodeDict.Add(setVar.GetGUID(), setVar);
            }
            foreach (var character in project.characters)
            {
                var enterCharNode = new EnterCharacterInSceneNode(character);
                AddElement(enterCharNode);
                nodeDict.Add(enterCharNode.GetGUID(), enterCharNode);
            }
            foreach (var dialogues in project.dialogues)
            {
                var dialNode = new DialogueNode(dialogues);
                AddElement(dialNode);
                nodeDict.Add(dialNode.GetGUID(), dialNode);
            }
            foreach (var switches in project.switches)
            {
                var switchNode = new DialogueSwitch(switches);
                AddElement(switchNode);
                nodeDict.Add(switchNode.GetGUID(), switchNode);
            }

            foreach (var reqVar in project.requiredVariables)
            {
                var reqNode = new RequirementNode(reqVar);
                AddElement(reqNode);
            }

            foreach (var node in nodes)
            {
                switch ((object)node)
                {
                    case EnterGraphNode egn:
                        {
                            string key = egn.entryNode.nextNodeGUID[0];
                            if (!string.IsNullOrEmpty(key))
                                AddElement(egn.outPort.ConnectTo(nodeDict[key].inPort));
                        }
                        break;
                    case SetVariableNode svn:
                        {
                            string key = svn.declareVariable.nextNodeGUID[0];
                            if (!string.IsNullOrEmpty(key))
                                AddElement(svn.outPort.ConnectTo(nodeDict[key].inPort));
                        }
                        break;
                    case EnterCharacterInSceneNode ecisn:
                        {
                            string key = ecisn.enterCharacterData.nextNodeGUID[0];
                            if (!string.IsNullOrEmpty(key))
                                AddElement(ecisn.outPort.ConnectTo(nodeDict[key].inPort));
                        }
                        break;
                    case DialogueNode dn:
                        {
                            for (int i = 0; i < dn.dialogue.dialogueOption.Count; i++)
                            {
                                string key = dn.dialogue.dialogueOption[i].NextNodeGUID;
                                if (!string.IsNullOrEmpty(key))
                                    AddElement(dn.outPorts[i].ConnectTo(nodeDict[key].inPort));
                            }
                        }
                        break;
                    case DialogueSwitch dsn:
                        {
                            string dkey = dsn.dialogueSwitchData.defaultNextNodeGUID;
                            if (!string.IsNullOrEmpty(dkey)) AddElement(dsn.defaultOutPort.ConnectTo(nodeDict[dkey].inPort));
                            for (int i = 0; i < dsn.switchOptions.Count; i++)
                            {
                                string key = dsn.dialogueSwitchData.nextNodes[i].nextNodeGUID;
                                if (!string.IsNullOrEmpty(key))
                                    AddElement(dsn.switchOptions[i].outPort.ConnectTo(nodeDict[key].inPort));
                            }
                        }
                        break;
                    case RequirementNode rn:
                        {
                            foreach (var connectedDials in rn.requirement.connectedDialogues)
                            {
                                string key = connectedDials.nodeGUID;
                                int index = connectedDials.index;
                                var nodeD = nodeDict[key];
                                if (nodeD is DialogueNode dNode)
                                {
                                    Port port = dNode.inPorts[index];
                                    AddElement(rn.outPort.ConnectTo(port));
                                }
                                else if (nodeD is DialogueSwitch dsNode)
                                {
                                    Port port = dsNode.switchOptions[index].inPort;
                                    AddElement(rn.outPort.ConnectTo(port));
                                }
                            }
                        }
                        break;
                }
            }
        }

        public bool CheckDialogueExistance()
        {
            return !(currentFile == null);
        }

        public void SaveFile()
        {
            if (!CheckDialogueExistance())
            {
                Debug.Log("No file selected please create new dialogue");
                return;
            }

            currentFile.variables.Clear();
            currentFile.characters.Clear();
            currentFile.dialogues.Clear();
            currentFile.switches.Clear();
            currentFile.requiredVariables.Clear();

            foreach (var node in nodes)
            {
                switch ((object)node)
                {
                    case EnterGraphNode egn:
                        {
                            egn.entryNode.position = egn.GetPosition();
                            currentFile.entryNode = egn.entryNode;
                        }
                        break;
                    case SetVariableNode svn:
                        {
                            svn.declareVariable.position = svn.GetPosition();
                            currentFile.variables.Add(svn.declareVariable);
                        }
                        break;
                    case EnterCharacterInSceneNode ecisn:
                        {
                            ecisn.enterCharacterData.position = ecisn.GetPosition();
                            currentFile.characters.Add(ecisn.enterCharacterData);
                        }
                        break;
                    case DialogueNode dn:
                        {
                            dn.dialogue.position = dn.GetPosition();
                            currentFile.dialogues.Add(dn.dialogue);
                        }
                        break;
                    case DialogueSwitch dsn:
                        {
                            dsn.dialogueSwitchData.position = dsn.GetPosition();
                            currentFile.switches.Add(dsn.dialogueSwitchData);
                        }
                        break;
                    case RequirementNode rn:
                        {
                            rn.requirement.position = rn.GetPosition();
                            currentFile.requiredVariables.Add(rn.requirement);
                        }
                        break;
                }
            }
            EditorUtility.SetDirty(currentFile);
            currentFile.UpdateDictionary();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Dialogue Saved");
        }
    }
}