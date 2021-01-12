using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
[DefaultExecutionOrder(-1000)]
public class PathNode : MonoBehaviour
{
    public enum NodeType
    {
        Path,
        Town,
        Dungeon,
        Secret,
    }

    [System.Flags]
    public enum Accomodies
	{
        None     = 0x00,
        Smith    = 0x01,
        Academy  = 0x02,
        Bank     = 0x04,
        Shop     = 0x08,
        Inn      = 0x10,
        Travel   = 0x20,
        Start    = 0x40, 
	}

    [System.Serializable]
    public class ConnectionInfo
	{
        public NodeType thisType;
        public List<PathNode> connectedNodes = new List<PathNode>();
	}

    [SerializeField] public ConnectionInfo connectionInfo = new ConnectionInfo();

    private Image myImage;

    public Accomodies accomodies;

    public static ConnectionInfo playerHome;

    public void Initialize()
    {
        myImage = GetComponent<Image>();
        myImage.color = (connectionInfo.thisType == NodeType.Path) ? Color.yellow : (connectionInfo.thisType == NodeType.Town) ? Color.cyan : (connectionInfo.thisType == NodeType.Dungeon) ? Color.red : Color.green; //(myImage.color == Color.red) ? Color.yellow : Color.red;

        for (int i = 0; i < connectionInfo.connectedNodes.Count; i++)
        {
            UpdateNodeConnections(connectionInfo.connectedNodes[i]);
        }

        if ((accomodies & Accomodies.Start) != 0) playerHome = connectionInfo; //if the start flag has been set

    }

#if false//UNITY_EDITOR
    private void Update()
	{
        myImage.color = (connectionInfo.thisType == NodeType.Path) ? Color.yellow : (connectionInfo.thisType == NodeType.Town) ? Color.cyan : (connectionInfo.thisType == NodeType.Dungeon) ? Color.red : Color.green; //(myImage.color == Color.red) ? Color.yellow : Color.red;
        for (int i = 0; i < connectionInfo.connectedNodes.Count; i++)
		{
            if (connectionInfo.connectedNodes[i] == null) { myImage.color = Color.magenta; return;  }
            if (!connectionInfo.connectedNodes[i].connectionInfo.connectedNodes.Contains(this)) connectionInfo.connectedNodes[i].connectionInfo.connectedNodes.Add(this);

            Debug.DrawLine(transform.position         , connectionInfo.connectedNodes[i].transform.position         , Color.red, 0,false);
            Debug.DrawLine(transform.position * 1.001f, connectionInfo.connectedNodes[i].transform.position * 1.001f, Color.red, 0,false);
            Debug.DrawLine(transform.position * 1.002f, connectionInfo.connectedNodes[i].transform.position * 1.002f, Color.red, 0,false);
            Debug.DrawLine(transform.position * 0.998f, connectionInfo.connectedNodes[i].transform.position * 0.998f, Color.red, 0,false);
            Debug.DrawLine(transform.position * 0.999f, connectionInfo.connectedNodes[i].transform.position * 0.999f, Color.red, 0,false);            
		}
    }
#endif
    

    void UpdateNodeConnections(PathNode node)
	{
        if (node == null) { return; }
        if (!node.connectionInfo.connectedNodes.Contains(this)) node.connectionInfo.connectedNodes.Add(this);
    }
}
