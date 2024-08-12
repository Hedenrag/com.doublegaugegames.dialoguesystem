using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class BaseNode : Node
{
    public abstract string GetGUID();

    public PortCall inPort;
    public BaseNode()
    {
        inPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
        //inPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
        inPort.portName = "Input";
        inputContainer.Add(inPort);
    }
}
