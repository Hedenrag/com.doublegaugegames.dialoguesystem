using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SetVariableNode : BaseNode
{
    public DeclareVariable declareVariable;
    public PortCall outPort;

    Button setButton;

    TextField textField;
    internal SetVariableNode() { }
    public SetVariableNode(Vector2 position)
    {
        declareVariable = new();
        declareVariable.VariableName = "VariableName";
        declareVariable.Set = true;
        SetPosition(new Rect(position, new Vector2(200, 150)));
        title = "";

        CreateVisuals();
    }

    public SetVariableNode(DeclareVariable variable)
    {
        declareVariable = variable;
        SetPosition(declareVariable.position);

        CreateVisuals() ;
    }

    void CreateVisuals()
    {
        title = "Set Variable";
        //outPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Output, PortCall.Capacity.Single, typeof(int));
        outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, PortCall.Capacity.Single, typeof(int));
        outPort.portName = "Output";
        outPort.OnConnect = UpdatePort;
        outPort.OnDisconnect = UpdatePort;
        outputContainer.Add(outPort);

        textField = new TextField() { label = "variable", value = declareVariable.VariableName };
        textField.RegisterValueChangedCallback(x => declareVariable.VariableName = x.newValue);
        mainContainer.Add(textField);

        setButton = new Button(() => { declareVariable.Set = !declareVariable.Set; setButton.text = declareVariable.Set ? "Register" : "Remove"; });
        setButton.text = declareVariable.Set ? "Register" : "Remove";
        titleButtonContainer.Add(setButton);
    }

    public void UpdateVariable()
    {
        declareVariable.position = GetPosition();
    }

    void UpdatePort(PortCall caller)
    {
        Edge connection = caller.connections.FirstOrDefault();
        if (connection == null) { declareVariable.nextNodeGUID[0] = null; }
        declareVariable.nextNodeGUID[0] = ((BaseNode)connection.input.node).GetGUID();
    }

    public override string GetGUID()
    {
        return declareVariable.GUID;
    }
}
