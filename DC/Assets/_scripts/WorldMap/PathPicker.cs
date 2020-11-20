using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPicker : MonoBehaviour
{
    [SerializeField] private PathNode previousNode;
    [SerializeField] private PathNode currentNode;

    private Button[] buts;
    private GameObject locationPointer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("REMEMBER TO PUT MAP DETAILS IN PROPER PLACE");
        buts = GameObject.Find("PathChoiceButtons").GetComponentsInChildren<Button>();
        locationPointer = GameObject.Find("LocationPointer");
        UpdateButs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
		{
            UpdateButs();
        }
    }

    void UpdateButs()
	{
        for (int i = 0; i < buts.Length; i++)
        {
            buts[i].onClick.RemoveAllListeners();
            buts[i].gameObject.SetActive(false);
        }


        print(currentNode.transform.name + ": " + currentNode.connectionInfo.connectedNodes.Count);
        for (int i = 0; i < currentNode.connectionInfo.connectedNodes.Count; i++)
        {
            var curNode = currentNode.connectionInfo.connectedNodes[i];
            if (curNode == previousNode && (currentNode.connectionInfo.thisType == PathNode.NodeType.Path || currentNode.connectionInfo.thisType == PathNode.NodeType.Secret)) continue;
            
            buts[i].GetComponentInChildren<Text>().text = curNode.transform.name;

            buts[i].onClick.AddListener(delegate { previousNode = currentNode; currentNode = curNode; UpdateButs(); locationPointer.transform.position = currentNode.transform.position; });
            buts[i].gameObject.SetActive(true);
        }
    }
}
