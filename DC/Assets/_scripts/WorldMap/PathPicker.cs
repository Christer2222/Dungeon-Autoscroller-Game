using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPicker : MonoBehaviour
{
    [SerializeField] private PathNode previousNode;
    [SerializeField] private PathNode currentNode;

    private List<PathNode> selectableNodes = new List<PathNode>();

    void Start()
    {
        UIController.WorldLocationPointer.position = currentNode.transform.position;

        UpdateSelectableNodes();
        UpdateButs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Options.mapKey))
		{
            UIController.WorldCanvas.gameObject.SetActive(!UIController.WorldCanvas.gameObject.activeSelf);
            UIController.PathChoiceButtonsHolder.gameObject.SetActive(!UIController.PathChoiceButtonsHolder.gameObject.activeSelf);

            UpdateButs();
		}

        if (Input.GetKeyDown(KeyCode.H))
		{
            GoToNextNode();
		}
    }

    void UpdateButs()
	{
        for (int i = 0; i < UIController.PathChoiceButtons.Length; i++)
        {
            UIController.PathChoiceButtons[i].onClick.RemoveAllListeners();
            UIController.PathChoiceButtons[i].gameObject.SetActive(false);
        }

        //print(currentNode.transform.name + ": " + currentNode.connectionInfo.connectedNodes.Count);
        for (int i = 0; i < selectableNodes.Count; i++ )//currentNode.connectionInfo.connectedNodes.Count; i++)
        {
            var curNode = selectableNodes[i];
            //var curNode = currentNode.connectionInfo.connectedNodes[i];
            //if (curNode == previousNode && (currentNode.connectionInfo.thisType == PathNode.NodeType.Path || currentNode.connectionInfo.thisType == PathNode.NodeType.Secret)) continue;

            UIController.PathChoiceButtons[i].GetComponentInChildren<Text>().text = curNode.transform.name;

            UIController.PathChoiceButtons[i].onClick.AddListener(delegate { 
                previousNode = currentNode; 
                currentNode = curNode;
                UpdateSelectableNodes();
                UpdateButs(); 
                UIController.WorldLocationPointer.position = currentNode.transform.position; 
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

    void GoToNextNode()
	{
        print("next node");
        if (selectableNodes.Count == 1)
		{
            previousNode = currentNode;
            currentNode = selectableNodes[0];
            UpdateSelectableNodes();
            UpdateButs();
            UIController.WorldLocationPointer.position = currentNode.transform.position;
        }
	}
}
