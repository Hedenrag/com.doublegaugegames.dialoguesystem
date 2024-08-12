using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;

namespace DGG.DialogueSystem
{

    public class RequirementNode : Node
    {
        internal RequireVariable requirement;

        internal PortCall outPort;

        internal RequirementNode() { }
        public RequirementNode(Vector2 position)
        {
            requirement = new();
            SetPosition(new Rect(position, new(200f, 150f)));
            InstanceNode();
        }
        public RequirementNode(RequireVariable requireVariable)
        {
            this.requirement = requireVariable;
            SetPosition(requireVariable.position);
            InstanceNode();
        }

        void InstanceNode()
        {
            title = "Requirement";
            //outPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            outPort.portName = "out";
            outPort.OnConnect = UpdatePort;
            outPort.OnDisconnect = UpdatePort;
            outputContainer.Add(outPort);

            var variable = new TextField() { label = "", value = requirement.variableName };
            variable.RegisterValueChangedCallback(x =>
            {
                requirement.variableName = x.newValue;
                //TODO update targets
                UpdatePort(outPort);
            });
            inputContainer.Add(variable);
        }

        void UpdatePort(PortCall caller)
        {
            requirement.connectedDialogues = new();
            foreach (var edge in caller.connections)
            {
                PortCall port = (PortCall)edge.input;
                var conNode = (BaseNode)edge.input.node;
                int index = 0;
                if (conNode is DialogueNode dNode)
                {
                    index = dNode.inPorts.IndexOf(port);
                }
                else if (conNode is DialogueSwitch dsNode)
                {
                    index = dsNode.switchOptions.FindIndex(x => x.inPort == port);
                }

                string guid = conNode.GetGUID();
                requirement.connectedDialogues.Add(new(guid, index));
            }
        }
    }
}