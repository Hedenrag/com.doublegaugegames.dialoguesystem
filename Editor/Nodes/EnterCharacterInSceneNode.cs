using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EnterCharacterInSceneNode : BaseNode
{
    public EnterCharacterData enterCharacterData;

    public PortCall outPort;

    List<NameNGUID> nameNGUIDs;

    Button buttonEnter;
    Button buttonSide;

    internal EnterCharacterInSceneNode() { }
    public EnterCharacterInSceneNode(Vector2 position)
    {
        enterCharacterData = new EnterCharacterData();
        SetPosition(new Rect(position, new Vector2(200f, 150f)));
        InitNode();
    }
    public EnterCharacterInSceneNode(EnterCharacterData data)
    {
        enterCharacterData = data;
        SetPosition(data.position);
        InitNode();
    }

    void InitNode()
    {
        title = "SetCharacter";

        //outPort = (PortCall)InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
        outPort = PortCall.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
        outPort.portName = "out";
        outputContainer.Add(outPort);
        outPort.OnConnect = UpdateNode; 
        outPort.OnDisconnect = UpdateNode;

        buttonEnter = new Button(() => 
        { 
            enterCharacterData.EnterCharacter = !enterCharacterData.EnterCharacter;
            buttonEnter.text = enterCharacterData.EnterCharacter ? "Enter" : "Exit"; 
        });
        buttonEnter.text = enterCharacterData.EnterCharacter ? "Enter" : "Exit";
        mainContainer.Add(buttonEnter);

        buttonSide = new Button(() =>
        {
            enterCharacterData.CharacterLeft = !enterCharacterData.CharacterLeft;
            buttonSide.text = enterCharacterData.CharacterLeft ? "Left" : "Right";
        });
        buttonSide.text = enterCharacterData.CharacterLeft ? "Left" : "Right";
        mainContainer.Add(buttonSide);

        DropdownField dropdown = new DropdownField();
        nameNGUIDs = enterCharacterData.CharacterDefinition.CharacterDefinitions.Select(x =>new NameNGUID(){name = x.name, guid = x.GUID}).ToList();
        dropdown.choices = nameNGUIDs.Select(x => x.name).ToList();
        if(string.IsNullOrEmpty(enterCharacterData.SpeakerCharacterGUID)) dropdown.index = -1;
        else dropdown.index = nameNGUIDs.FindIndex(x => x.guid == enterCharacterData.SpeakerCharacterGUID);
        dropdown.RegisterValueChangedCallback(value => { enterCharacterData.SpeakerCharacterGUID = nameNGUIDs[dropdown.index].guid; });
        dropdown.label = "Character";
        mainContainer.Add(dropdown);

        //TODO pose
    }

    struct NameNGUID
    {
        public string name;
        public string guid;
    }

    void UpdateNode(PortCall caller)
    {
        Edge connection = caller.connections.FirstOrDefault();
        if (connection == null) { enterCharacterData.nextNodeGUID[0] = null; }
        enterCharacterData.nextNodeGUID[0] = ((BaseNode)connection.input.node).GetGUID();
    }

    public override string GetGUID()
    {
        return enterCharacterData.GUID;
    }
}
