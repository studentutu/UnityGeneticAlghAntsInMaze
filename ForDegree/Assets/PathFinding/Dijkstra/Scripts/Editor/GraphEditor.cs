using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{

    protected Graph m_Graph;
    protected NodeMono m_From;
    protected NodeMono m_To;
    protected Follower m_Follower;
    protected Path m_Path = new Path();

    void OnEnable()
    {
        m_Graph = target as Graph;
    }

    void OnSceneGUI()
    {
        if (m_Graph == null)
        {
            return;
        }
        Color before = Handles.color;
        Handles.color = Color.black;
        for (int i = 0; i < m_Graph.nodes.Count; i++)
        {
            Node node = m_Graph.nodes[i];
            for (int j = 0; j < node.connections.Count; j++)
            {
                Node connection = node.connections[j];
                if (connection == null)
                {
                    continue;
                }
                float distance = Vector3.Distance(node.position, connection.position);
                Vector3 diff = connection.position - node.position;
                Handles.Label(node.position + (diff / 2), distance.ToString(), EditorStyles.whiteLabel);
                if (m_Path.nodes.Contains(node) && m_Path.nodes.Contains(connection))
                {
                    Color color = Handles.color;
                    Handles.color = Color.green;
                    
                    Handles.DrawLine(node.position, connection.position);
                    Handles.color = color;
                }
                else
                {
                    Handles.DrawLine(node.position, connection.position);
                }
            }
        }
        Handles.color = before;
    }

    public override void OnInspectorGUI()
    {
        if (m_Graph.transform.childCount > 0)
        {
            m_Graph.nodes.Clear();
            foreach (Transform child in m_Graph.transform)
            {
                NodeMono node = child.GetComponent<NodeMono>();
                if (node != null)
                {
                    node.UpdateList();
                    m_Graph.nodes.Add(node.MyNode);
                }
            }
        }
        base.OnInspectorGUI();
        EditorGUILayout.Separator();
        m_From = (NodeMono)EditorGUILayout.ObjectField("From", m_From, typeof(NodeMono), true);
        m_To = (NodeMono)EditorGUILayout.ObjectField("To", m_To, typeof(NodeMono), true);


        m_Follower = (Follower)EditorGUILayout.ObjectField("Follower", m_Follower, typeof(Follower), true);
        if (GUILayout.Button("Show Shortest Path"))
        {
            if (m_From == null || m_To == null)
            {
				m_Path = m_Graph.findFromNodesGiven();
            }
            else
            {
                m_Path = m_Graph.GetShortestPath(m_From.MyNode, m_To.MyNode);
            }
            if (m_Follower != null)
            {
                m_Follower.Follow(m_Path);
            }
            Debug.Log(m_Path);
            SceneView.RepaintAll();
        }
    }

}
