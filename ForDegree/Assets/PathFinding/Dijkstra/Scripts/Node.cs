using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Node.
/// </summary>
public class Node
{
    public Vector3 position;
    public string name
    {
        get
        {
            return position.ToString();
        }
    }

    /// <summary>
    /// The connections (neighbors).
    /// </summary>
    public List<Node> m_Connections = new List<Node>();

    /// <summary>
    /// Gets the connections (neighbors).
    /// </summary>
    /// <value>The connections.</value>
    public List<Node> connections
    {
        get
        {
            return m_Connections;
        }
    }


    public void AddNeighbor(Node neigbor)
    {
        m_Connections.Add(neigbor);
        m_Connections = m_Connections.Distinct().ToList();

    }

}
