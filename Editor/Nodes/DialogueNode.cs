using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using static DialogueData;

public class DialogueNode : BaseNode
{
    public DialogueData dialogue;

    public List<PortCall> outPorts = new();
    public List<PortCall> inPorts = new();
    public List<VisualElement> options = new();

    DropdownField characterDropdown;
    Button sideButton;

    internal DialogueNode() { }
    public DialogueNode(Vector2 position)
    {
        dialogue = new DialogueData();
        SetPosition(new Rect(position, new(200f, 150f)));
        InitDialogue();
    }
    public DialogueNode(DialogueData data)
    {
        dialogue = data;
        SetPosition(dialogue.position);
        InitDialogue();
    }

    void InitDialogue()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogEditorStyles"));
        title = "Dialogue Node";

        RefreshExpandedState();
        RefreshPorts();

        var visualChar = new VisualElement();

        List<string> optionsList = dialogue.CharacterDefinition.CharacterDefinitions.Select(x => x.name).ToList();
        characterDropdown = new DropdownField("character", optionsList, -1);
        characterDropdown.RegisterValueChangedCallback(x =>
        {
            dialogue.characterGUID = dialogue.CharacterDefinition.CharacterDefinitions.Where(y => x.newValue == y.name).First().GUID;
        });
        characterDropdown.name = "CharacterSideButton";
        visualChar.Add(characterDropdown);
        visualChar.name = "VisualPortContainer";

        sideButton = new Button(() => { dialogue.characterSide = !dialogue.characterSide; sideButton.text = dialogue.characterSide ? "right" : "left"; });
        sideButton.text = dialogue.characterSide ? "right" : "left";
        sideButton.name = "CaracterChooseField";
        visualChar.Add(sideButton);

        mainContainer.Add(visualChar);

        var textField = new TextField("");
        textField.multiline = true;
        textField.RegisterValueChangedCallback(x =>
        {
            dialogue.text = x.newValue;
        });
        textField.SetValueWithoutNotify(dialogue.text);
        mainContainer.Add(textField);

        var button = new Button(() => AddOutPort()) {text = "Add Choice" };
        titleButtonContainer.Add(button);

        for(int i = 0; i < dialogue.dialogueOption.Count; i++)
        {
            var option = dialogue.dialogueOption[i];
            AddOutPort(option.optionName, option.answer, option.NextNodeGUID, i);
        }
    }

    public void AddOutPort(string text = null, string answer = null, string nextNodeGUID = null, int index = 0)
    {
        if(text == null) 
        {
            dialogue.dialogueOption.Add(new DialogueOption(dialogue));
            index = dialogue.dialogueOption.Count - 1;
        }
        DialogueOption dialOpt = dialogue.dialogueOption[index];
        dialOpt.answer = answer;
        dialOpt.NextNodeGUID = nextNodeGUID;

        PortCall outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
        //PortCall outPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
        outPort.portName = dialOpt.optionName;
        outPort.OnConnect = UpdateOutPort;
        outPort.OnDisconnect = UpdateOutPort;
        outPorts.Add(outPort);
        
        //PortCall inPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        PortCall inPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inPort.portName = dialOpt.optionName;
        inPort.OnConnect = UpdateInPort;
        inPort.OnDisconnect = UpdateInPort;
        inPorts.Add(inPort);

        inputContainer.Add(inPort);
        outputContainer.Add(outPort);

        VisualElement visualElement = new();
        visualElement.name = "VisualPortContainer";

        var NameField = new TextField("");
        NameField.RegisterValueChangedCallback(evt => { dialOpt.optionName = evt.newValue; outPort.portName = dialOpt.optionName; inPort.portName = dialOpt.optionName; });
        NameField.SetValueWithoutNotify(dialOpt.optionName);
        NameField.name = "NameField";

        var OptionText = new TextField("");
        OptionText.RegisterValueChangedCallback(evt => { dialOpt.answer = evt.newValue; });
        OptionText.SetValueWithoutNotify(dialOpt.answer);
        OptionText.name = "OptionText";

        var Button = new Button(() => { RemovePort(dialogue.dialogueOption.IndexOf(dialOpt)); }) { text = "X" };
        Button.name = "DeleteButton";

        visualElement.Add(NameField);
        visualElement.Add(OptionText);
        visualElement.Add(Button);
        options.Add(visualElement);
        mainContainer.Add(visualElement);

        RefreshExpandedState();
        RefreshPorts();
    }

    void UpdateOutPort(PortCall caller)
    {
        Edge connection = caller.connections.FirstOrDefault();
        var dOption = dialogue.dialogueOption[outPorts.IndexOf(caller)];
        if (connection == null) { dOption.NextNodeGUID = null; }
        else { dOption.NextNodeGUID = ((BaseNode)connection.input.node).GetGUID(); }
    }

    void UpdateInPort(PortCall caller)
    {
        List<string> requirements = new();
        int index = inPorts.IndexOf(caller);
        foreach(var connection in caller.connections)
        {
            requirements.Add(((RequirementNode)connection.output.node).requirement.variableName);
        }
        dialogue.dialogueOption[index].requirements = requirements.ToArray();
    }

    public void RemovePort(int ID)
    {
        var window = EditorWindow.GetWindow<DialogueEditorWindow>();
        foreach (var edge in outPorts[ID].connections)
        {
            edge.input.Disconnect(edge);
            window.graphView.RemoveElement(edge);
        }
        foreach (var edge in inPorts[ID].connections)
        {
            edge.input.Disconnect(edge);
            window.graphView.RemoveElement(edge);
        }
        mainContainer.Remove(options[ID]);
        outputContainer.Remove(outPorts[ID]);
        inputContainer.Remove(inPorts[ID]);

        outPorts.RemoveAt(ID);
        inPorts.RemoveAt(ID);
        options.RemoveAt(ID);
        dialogue.dialogueOption.RemoveAt(ID);

        RefreshExpandedState();
        RefreshPorts();
    }

    public override string GetGUID()
    {
        return dialogue.GUID;
    }

    
}
