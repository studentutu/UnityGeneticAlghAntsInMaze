using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class NodeMono : MonoBehaviour
{

    public Node MyNode = new Node();  // inner connection should be mannualy updated!
    [SerializeField] public List<NodeMono> connections = new List<NodeMono>();// will be filled in Maze Generation

    [Header(" For Individual")]
    [SerializeField] private bool isIndividual;

    // Called once for Floor tiles
    public void UpdateList()
    {
        connections = connections.Distinct().ToList();
        MyNode.position = transform.position;


        foreach (var item in connections)
        {
            if (item != null)
            {
                MyNode.m_Connections.Add(item.MyNode);
            }
        }

        // Removing duplicate elements
        MyNode.m_Connections = MyNode.m_Connections.Distinct().ToList();

    }

    public void Awake()
    {
        MyNode.position = transform.position;
        if (isIndividual)
        {
            Graph.Instance.nodes.Add(MyNode);
        }
    }
    // public void Update()  // non optimized version
    // {
    //     if (isIndividual )
    //     {
    //         connections.Clear();
    //         MyNode.connections.Clear();
    //         // Update position
    //         MyNode.position = transform.position;

    //         // Update Connections
    //         var newOne = transform.position;
    //         var reference = MazeSpawner.Instance;

    //         int row = (int)((newOne.z + reference.ZfloatDistanceCellHeight / 2) / (reference.ZfloatDistanceCellHeight)); // heaight
    //         int colums = (int)((newOne.x + reference.XfloatDistanceCellWidth / 2) / (reference.XfloatDistanceCellWidth)); // widht

    //         MazeCell mazeCell = reference.wholeMaze[row, colums];

    //         // at this point in time, floor tile will have it's connections marked!
    //         connections.Add(mazeCell.myMonoCell);
    //         foreach (var item in mazeCell.neighbor)
    //         {
    //             connections.Add(item.myMonoCell);
    //             MyNode.connections.Add(item.myMonoCell.MyNode);
    //         }

    //     }
    // }
}
