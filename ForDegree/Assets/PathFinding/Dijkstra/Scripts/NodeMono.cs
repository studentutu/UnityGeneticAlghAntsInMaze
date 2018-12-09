using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class NodeMono : MonoBehaviour
{

    public Node MyNode = new Node();  // inner connection should be mannualy updated!
    [SerializeField] public List<NodeMono> connections = new List<NodeMono>();
    
    
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
    }

}
