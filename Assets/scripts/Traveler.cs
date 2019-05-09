using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A traveler
/// </summary>
public class Traveler : MonoBehaviour
{
    [SerializeField]
    GameObject explosionPrefab;
    // events fired by class
    PathFoundEvent pathFoundEvent = new PathFoundEvent();
    PathTraversalCompleteEvent pathTraversalCompleteEvent = new PathTraversalCompleteEvent();

    LinkedList<Waypoint> shortestPath = new LinkedList<Waypoint>();
    LinkedList<Waypoint> visitednodes = new LinkedList<Waypoint>();
    const float BaseImpulseForceMagnitude = 2.0f;
    Rigidbody2D rb2d;
    
  
    
    /// <summary>
    // Use this for initialization
    /// </summary>
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        EventManager.AddPathFoundInvoker(this);
        EventManager.AddPathTraversalCompleteInvoker(this);
        shortestPath = FindPath();
        
        StartMoving();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Adds the given listener for the PathFoundEvent
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddPathFoundListener(UnityAction<float> listener)
    {
        pathFoundEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the PathTraversalCompleteEvent
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddPathTraversalCompleteListener(UnityAction listener)
    {
        pathTraversalCompleteEvent.AddListener(listener);
    }

    public LinkedList<Waypoint> FindPath()
    {
        Graph<Waypoint> graph = GraphBuilder.Graph;
        IList<GraphNode<Waypoint>> nodes = graph.Nodes;

        
        SortedLinkedList<SearchNode<Waypoint>> searchList =
                            new SortedLinkedList<SearchNode<Waypoint>>();


        Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>> dic =
                            new Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>>();

        Waypoint start = nodes[0].Value;
        Waypoint end = nodes[1].Value;
        LinkedList<Waypoint> path = new LinkedList<Waypoint>();

        foreach (GraphNode<Waypoint> graphNode in nodes)
        {
            SearchNode<Waypoint> searchNode = new SearchNode<Waypoint>(graphNode);
            if (graphNode.Value == start)
                searchNode.Distance = 0;
            searchList.Add(searchNode);
            dic.Add(graphNode, searchNode);
        }

        while (searchList.Count != 0)
        {

            SearchNode<Waypoint> currentSearchNode = searchList.First.Value;
            searchList.RemoveFirst();

            
            GraphNode<Waypoint> currentGraphNode = currentSearchNode.GraphNode;
            dic.Remove(currentGraphNode);

            
            if (currentSearchNode.GraphNode.Value == end)
            {
                //generating the path;
                path.AddFirst(currentSearchNode.GraphNode.Value);

                SearchNode<Waypoint> currentnode = currentSearchNode;

                while (currentnode.Previous != null)
                {

                    Debug.Log("previus changed to" + currentnode.GraphNode.Value.Id);
                    currentnode = currentnode.Previous;
                    Debug.Log("added " + currentnode.GraphNode.Value.Id);
                    path.AddFirst(currentnode.GraphNode.Value);
                    visitednodes.AddFirst(currentnode.GraphNode.Value);
                }

                visitednodes.RemoveFirst();
                pathFoundEvent.Invoke(currentSearchNode.Distance);

            }

            foreach (GraphNode<Waypoint> neighbour in currentGraphNode.Neighbors)
            {
                if (dic.ContainsKey(neighbour))
                {
                    
                    float distance = currentSearchNode.Distance +
                                              currentGraphNode.GetEdgeWeight(neighbour);

                    if (distance < dic[neighbour].Distance)
                    {
                        dic[neighbour].Distance = distance;
                        dic[neighbour].Previous = currentSearchNode;
                        searchList.Reposition(dic[neighbour]);
                    }
                }
            }

        }
        LinkedListNode<Waypoint> pathnode = path.First;
        
        return path;
    }

    public void StartMoving()
    {
        if (shortestPath.First != null)
        {
            Waypoint target = shortestPath.First.Value;

            Vector2 direction = new Vector2(
            target.Position.x - transform.position.x,
            target.Position.y - transform.position.y);
            direction.Normalize();
            rb2d.velocity = Vector2.zero;

            rb2d.AddForce(direction * BaseImpulseForceMagnitude,
                ForceMode2D.Impulse);
            shortestPath.RemoveFirst();
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            pathTraversalCompleteEvent.Invoke();
            
            DestroyWaypoints();

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartMoving();
    }

    void DestroyWaypoints()
    {
        LinkedListNode<Waypoint> previousNode = visitednodes.First;

        LinkedListNode < Waypoint > currentNode = previousNode.Next;
        
        Vector3 position = previousNode.Value.gameObject.transform.position;
        Instantiate(explosionPrefab, position, Quaternion.identity);
        Destroy(previousNode.Value.gameObject);

        while (currentNode != null)
        {
            previousNode = currentNode;
            currentNode = currentNode.Next;
            position = previousNode.Value.gameObject.transform.position;
            Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(previousNode.Value.gameObject);
        }

    }
}


