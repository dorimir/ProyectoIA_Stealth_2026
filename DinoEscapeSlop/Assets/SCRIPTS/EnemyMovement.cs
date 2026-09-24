using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private AStarPathfinding pathfinding;
    private List<Vector3> currentPath;
    private int targetIndex;

    public float speed = 5f;

    void Start()
    {
        pathfinding = FindObjectOfType<AStarPathfinding>();
        GoToRandomNode();
    }

    void Update()
    {
        FollowPath();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (pathfinding == null) return;

        currentPath = pathfinding.FindPath(transform.position, targetPosition);
        if (currentPath != null && currentPath.Count > 0)
        {
            targetIndex = 0;
        }
        else
        {
            // Si el A* no encuentra camino, intentamos con otro nodo aleatorio inmediatamente
            Invoke("GoToRandomNode", 1f);
        }
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector3 currentWaypoint = currentPath[targetIndex];

        // Nos movemos hacia el nodo
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

        // Aumentamos ligeramente el margen a 0.2f para evitar que falle por precisión numérica
        if (Vector3.Distance(transform.position, currentWaypoint) < 0.2f)
        {
            targetIndex++;
            if (targetIndex >= currentPath.Count)
            {
                currentPath = null;
                // Espera 0.5 segundos antes de elegir el siguiente nodo para que parezca natural
                Invoke("GoToRandomNode", 0.5f);
            }
        }
    }

    void GoToRandomNode()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        if (allNodes.Length > 0)
        {
            Node randomNode = allNodes[Random.Range(0, allNodes.Length)];
            MoveTo(randomNode.transform.position);
        }
    }
}