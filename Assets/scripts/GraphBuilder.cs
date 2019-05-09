using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the graph
/// </summary>
public class GraphBuilder : MonoBehaviour
{
    static Graph<Waypoint> graph;

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    void Awake()
    {
        graph = new Graph<Waypoint>();
        // add nodes (all waypoints, including start and end) to graph
        Debug.Log("awake method called");
        //adding start node

        Waypoint start = GameObject.FindGameObjectWithTag("Start").GetComponent<Waypoint>();
        Debug.Log("start no : " + start.Id);

        graph.AddNode(start);

        Debug.Log("added start");
        //adding last node
        IList<GraphNode<Waypoint>> nodes = graph.Nodes;
        string a = "";

        foreach (GraphNode<Waypoint> entry in nodes)
        {
            a = a + ", " + entry.Value.Id;
        }
        Debug.Log(" nodes =   " + a);
        a = "";


        graph.AddNode(GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>());

        nodes = graph.Nodes;
        foreach (GraphNode<Waypoint> entry in nodes)
        {
            a = a + ", " + entry.Value.Id;
        }
        Debug.Log(" nodes =   " + a);
        a = "";


        GameObject[] arr = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach( GameObject entry in arr)
        {
            graph.AddNode(entry.GetComponent<Waypoint>());
        }

        nodes = graph.Nodes;

        foreach (GraphNode<Waypoint> entry in nodes)
        {
            a = a + ", " + entry.Value.Id;
        }
        Debug.Log(" nodes =   " + a);
        a = "";

        IList<GraphNode<Waypoint>> temp = graph.Nodes;
        for (int i = 0; i < temp.Count-1; i++)
        {
            Waypoint nodeA = temp[i].Value;
            for (int j = i+1; j < temp.Count; j++)
            {
                Waypoint nodeB = temp[j].Value;
                if (Mathf.Abs(nodeA.Position.x - nodeB.Position.x) < 3.5 && Mathf.Abs(nodeA.Position.y - nodeB.Position.y)<3)
                {
                    graph.AddEdge(nodeA, nodeB, Vector2.Distance(nodeA.Position, nodeB.Position));
                    
                }

            }
        }

    }

    /// <summary>
    /// Gets the graph
    /// </summary>
    /// <value>graph</value>
    public static Graph<Waypoint> Graph
    {
        get { return graph; }
    }
}
