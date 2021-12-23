using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPicker : MonoBehaviour
{
    [SerializeField] private PathNode previousNode;
    [SerializeField] public PathNode currentNode;

    [SerializeField] public PathNode testNextNode;

    private List<PathNode> selectableNodes = new List<PathNode>();

    public static PathPicker instance;

    void Start()
    {
        instance = this;
        UIController.WorldLocationMarker.position = currentNode.transform.position; //places a marker at the current node

        UpdateSelectableNodes();
        UpdatePathChoiceButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Options.mapKey))
		{
            UIController.WorldCanvas.gameObject.SetActive(!UIController.WorldCanvas.gameObject.activeSelf);
            //UIController.PathChoiceButtonsHolder.gameObject.SetActive(!UIController.PathChoiceButtonsHolder.gameObject.activeSelf);

            UpdatePathChoiceButtons();
		}
    }

    void UpdatePathChoiceButtons()
	{
        //when clicking a choice remove all options
        for (int i = 0; i < UIController.PathChoiceButtons.Length; i++)
        {
            UIController.PathChoiceButtons[i].onClick.RemoveAllListeners();
            UIController.PathChoiceButtons[i].gameObject.SetActive(false);
        }

        //if there are no options for the next node
        if (selectableNodes.Count == 1) 
		{
            testNextNode = selectableNodes[0];
            return;
		}

        for (int i = 0; i < selectableNodes.Count; i++ )
        {
            var curNode = selectableNodes[i]; //shortcut

            //give the button a name equal to the description it has
            UIController.PathChoiceButtons[i].GetComponentInChildren<Text>().text = curNode.description;

            //all choice buttons should do this when clicked
            UIController.PathChoiceButtons[i].onClick.AddListener(delegate { 
                previousNode = currentNode; //the node the player is on is now the last one
                currentNode = curNode; //and the picked one is the current
                UpdateSelectableNodes(); //update available paths
                UpdatePathChoiceButtons(); //then deactivate choice buttons (or set to new choices)
                UIController.WorldLocationMarker.position = currentNode.transform.position; //moves the marker to the new node

                UIController.PathChoiceButtonsHolder.gameObject.SetActive(false); //all choices disables previous options when clicked

                EncounterController.instance.RemoveFlagFromGameState(EncounterController.GameState.Waiting_For_Path); //remove the waiting for path flag
            });

            UIController.PathChoiceButtons[i].gameObject.SetActive(true); //allow the choice to be made, by activating the button

        }

        EncounterController.instance.AddFlagFromGameState(EncounterController.GameState.Waiting_For_Path);
    }

    /// <summary>
    /// Goes through all nodes connected, and updates the variable
    /// </summary>
    void UpdateSelectableNodes()
	{
        selectableNodes = currentNode.connectionInfo.connectedNodes.FindAll(x =>
        (x == previousNode && (currentNode.connectionInfo.thisType == PathNode.NodeType.Dungeon || currentNode.connectionInfo.thisType == PathNode.NodeType.Town)) //if node is the previous one, and was a checkpoint
        || (x != previousNode) //or if node was not the previous one
        );
    }

    /// <summary>
    /// Move map to next node.
    /// Returns true if only 1 next node exists.
    /// </summary>
    public int GoToNextNode()
	{
        print("next node");
        if (selectableNodes.Count == 1)
        {
            previousNode = currentNode;
            currentNode = selectableNodes[0];
            UpdateSelectableNodes();
            UpdatePathChoiceButtons();
            UIController.WorldLocationMarker.position = currentNode.transform.position; //moves the marker to the next node
        }
        else
            EncounterController.instance.AddFlagFromGameState(EncounterController.GameState.Waiting_For_Path);

        return selectableNodes.Count;
	}
}
