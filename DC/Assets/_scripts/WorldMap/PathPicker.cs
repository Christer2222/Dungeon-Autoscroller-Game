using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPicker : MonoBehaviour
{
    [SerializeField] private PathNode previousNode;
    [SerializeField] private PathNode currentNode;

    private List<PathNode> selectableNodes = new List<PathNode>();

    public static PathPicker instance;

    void Start()
    {
        instance = this;
        UIController.WorldLocationPointer.position = currentNode.transform.position;

        UpdateSelectableNodes();
        UpdatePathChoiceButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Options.mapKey))
		{
            UIController.WorldCanvas.gameObject.SetActive(!UIController.WorldCanvas.gameObject.activeSelf);
            UIController.PathChoiceButtonsHolder.gameObject.SetActive(!UIController.PathChoiceButtonsHolder.gameObject.activeSelf);

            UpdatePathChoiceButtons();
		}
    }

    void UpdatePathChoiceButtons()
	{
        for (int i = 0; i < UIController.PathChoiceButtons.Length; i++)
        {
            UIController.PathChoiceButtons[i].onClick.RemoveAllListeners();
            UIController.PathChoiceButtons[i].gameObject.SetActive(false);
        }

        if (selectableNodes.Count == 1)
            return;

        //print(currentNode.transform.name + ": " + currentNode.connectionInfo.connectedNodes.Count);
        for (int i = 0; i < selectableNodes.Count; i++ )//currentNode.connectionInfo.connectedNodes.Count; i++)
        {
            var curNode = selectableNodes[i];
            //var curNode = currentNode.connectionInfo.connectedNodes[i];
            //if (curNode == previousNode && (currentNode.connectionInfo.thisType == PathNode.NodeType.Path || currentNode.connectionInfo.thisType == PathNode.NodeType.Secret)) continue;

            UIController.PathChoiceButtons[i].GetComponentInChildren<Text>().text = curNode.description;//.transform.name;

            UIController.PathChoiceButtons[i].onClick.AddListener(delegate { 
                previousNode = currentNode; 
                currentNode = curNode;
                UpdateSelectableNodes();
                UpdatePathChoiceButtons(); 
                UIController.WorldLocationPointer.position = currentNode.transform.position;

                EncounterController.instance.RemoveFlagFromGameState(EncounterController.GameState.Waiting_For_Path); //remove the waiting for path flag
            });

            UIController.PathChoiceButtons[i].gameObject.SetActive(true);
        }
    }


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
            UIController.WorldLocationPointer.position = currentNode.transform.position;
        }
        else
            EncounterController.instance.AddFlagFromGameState(EncounterController.GameState.Waiting_For_Path);

        return selectableNodes.Count;
	}
}
