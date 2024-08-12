using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DGG.DialogueSystem
{
    public class DialogueSwitch : BaseNode
    {
        internal PortCall defaultOutPort;
        internal List<SwitchOption> switchOptions = new();

        Button addOptionButton;
        internal DialogueSwitchData dialogueSwitchData;

        public DialogueSwitch() { }

        public DialogueSwitch(Vector2 position)
        {
            dialogueSwitchData = new DialogueSwitchData();
            dialogueSwitchData.position = new Rect(position, new(200f, 150f));
            InitSwitch();
        }

        public DialogueSwitch(DialogueSwitchData dialogueSwitchData)
        {
            this.dialogueSwitchData = dialogueSwitchData;
            InitSwitch();
        }

        void InitSwitch()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogEditorStyles"));
            SetPosition(dialogueSwitchData.position);
            title = "Switch";

            defaultOutPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
            defaultOutPort.portName = "Default";
            defaultOutPort.OnConnect = UpdateDefaultPort;
            outputContainer.Add(defaultOutPort);

            addOptionButton = new Button(() => AddSwitchPort());
            addOptionButton.text = "Add";
            titleButtonContainer.Add(addOptionButton);

            //TODO add more options
            for (int i = 0; i < dialogueSwitchData.nextNodes.Count; i++)
            {
                var option = dialogueSwitchData.nextNodes[i];
                AddSwitchPort(option.nextNodeGUID, i);
            }
        }

        public void AddSwitchPort(string nextNodeGUID = null, int index = 0)
        {
            if (nextNodeGUID == null)
            {
                dialogueSwitchData.nextNodes.Add(new NextNode(null, string.Empty));
                index = dialogueSwitchData.nextNodes.Count - 1;
                nextNodeGUID = string.Empty;
            }
            NextNode nextNode = dialogueSwitchData.nextNodes[index];
            nextNode.nextNodeGUID = nextNodeGUID;

            //logic for adding options
            SwitchOption switchOption = new SwitchOption();
            switchOptions.Add(switchOption);

            switchOption.rightSide = new VisualElement();
            switchOption.rightSide.name = "VisualPortContainer";

            switchOption.deleteButton = new Button(() => RemovePort(switchOptions.IndexOf(switchOption)));
            switchOption.deleteButton.text = "x";
            switchOption.deleteButton.name = "DeleteButton";

            switchOption.outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
            switchOption.outPort.portName = "Out";
            switchOption.outPort.name = "NameField";
            switchOption.outPort.OnConnect = UpdateOutPort;
            switchOption.outPort.OnDisconnect = UpdateOutPort;

            switchOption.rightSide.Add(switchOption.deleteButton);
            switchOption.rightSide.Add(switchOption.outPort);

            switchOption.inPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            switchOption.inPort.portName = "Req";
            switchOption.inPort.OnConnect = UpdateInPort;
            switchOption.inPort.OnDisconnect = UpdateInPort;

            inputContainer.Add(switchOption.inPort);
            outputContainer.Add(switchOption.rightSide);
        }

        void RemovePort(int index)
        {
            var switchOption = switchOptions[index];
            var window = EditorWindow.GetWindow<DialogueEditorWindow>();

            foreach (var edge in switchOption.inPort.connections)
            {
                edge.input.Disconnect(edge);
                window.graphView.RemoveElement(edge);
            }
            foreach (var edge in switchOption.outPort.connections)
            {
                edge.input.Disconnect(edge);
                window.graphView.RemoveElement(edge);
            }

            inputContainer.Remove(switchOption.inPort);
            outputContainer.Remove(switchOption.rightSide);

            switchOptions.RemoveAt(index);
            Debug.Log("Removed switch port");
        }

        void UpdateDefaultPort(PortCall caller)
        {
            Edge connection = caller.connections.FirstOrDefault();
            if (connection == null) { dialogueSwitchData.defaultNextNodeGUID = null; return; }
            dialogueSwitchData.defaultNextNodeGUID = ((BaseNode)connection.input.node).GetGUID();
        }

        void UpdateOutPort(PortCall caller)
        {
            Edge connection = caller.connections.FirstOrDefault();
            int index = switchOptions.FindIndex(x => x.outPort == caller);
            NextNode workingNode = dialogueSwitchData.nextNodes[index];
            if (connection == null) { workingNode = null; return; }
            dialogueSwitchData.nextNodes[index].nextNodeGUID = ((BaseNode)connection.input.node).GetGUID();
        }

        void UpdateInPort(PortCall caller)
        {
            int index = switchOptions.FindIndex(x => x.inPort == caller);
            NextNode workingNode = dialogueSwitchData.nextNodes[index];
            List<string> requirements = new();
            foreach (var connection in caller.connections)
            {
                requirements.Add(((RequirementNode)connection.output.node).requirement.variableName);
            }
            dialogueSwitchData.nextNodes[index].requirements = requirements.ToArray();
        }

        public override string GetGUID()
        {
            return dialogueSwitchData.GUID;
        }

        internal class SwitchOption
        {
            public PortCall inPort;
            public PortCall outPort;
            public Button deleteButton;
            public VisualElement rightSide;
        }
    }
}