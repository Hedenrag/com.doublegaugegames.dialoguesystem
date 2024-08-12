using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnterGraphNode : Node
{
    public EntryNode entryNode;

    public PortCall outPort;

    public EnterGraphNode(EntryNode entryNode)
    {
        this.entryNode = entryNode;
        if(entryNode == null) { this.entryNode = new EntryNode(); }
        title = "START";
        //outPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Output, PortCall.Capacity.Single, typeof(int));
        outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, PortCall.Capacity.Single, typeof(int));
        outPort.name = "ENTRYPOINT";
        outPort.portName = "ENTRYPOINT";
        outPort.OnConnect = UpdateConnections;
        outPort.OnDisconnect = UpdateConnections;
        outputContainer.Add(outPort);
        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;
        RefreshExpandedState();
        RefreshPorts();
    }

    void UpdateConnections(PortCall caller)
    {
        Edge connection = caller.connections.FirstOrDefault();
        if (connection == null) { entryNode.nextNodeGUID[0] = null; return; }
        entryNode.nextNodeGUID[0] = ((BaseNode)connection.input.node).GetGUID();
    }
}
