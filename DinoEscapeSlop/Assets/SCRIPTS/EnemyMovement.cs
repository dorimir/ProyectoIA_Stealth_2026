using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private AStarPathfinding pathfinding;
    private List<Vector3> currentPath;
    private int targetIndex;

    public float speed = 5f;

    [Header("Nodos del Enemigo")]
    [Tooltip("Asigna aquí en el Inspector únicamente los nodos entre los que quieres que se mueva este enemigo.")]
    public List<Node> patrolNodes = new List<Node>();

    // Indica a la máquina de estados si el enemigo completó su trayecto
    public bool HasReachedDestination { get; private set; }

    void Start()
    {
        pathfinding = FindObjectOfType<AStarPathfinding>();

        // Marcamos true al inicio para que el FlockingState elija el primer destino en su primer frame
        HasReachedDestination = true;
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
            HasReachedDestination = false;
        }
        else
        {
            // Si el nodo elegido por azar dio un camino invalido/nulo, reintenta buscar otro
            HasReachedDestination = true;
            Invoke("GoToRandomNode", 0.5f);
        }
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector3 currentWaypoint = currentPath[targetIndex];

        // Mover hacia el nodo waypoint actual
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

        // Comprobación de llegada al waypoint actual
        if (Vector3.Distance(transform.position, currentWaypoint) < 0.2f)
        {
            targetIndex++;

            // ¿Llegamos al final del camino completo?
            if (targetIndex >= currentPath.Count)
            {
                currentPath = null;
                HasReachedDestination = true; // Notificamos que hemos llegado al destino

                Invoke("GoToRandomNode", 0.5f);
            }
        }
    }

    /// <summary>
    /// Selecciona un nodo aleatorio ÚNICAMENTE de la lista de nodos asignada en el Inspector.
    /// </summary>
    public void GoToRandomNode()
    {
        if (patrolNodes != null && patrolNodes.Count > 0)
        {
            // Elige un nodo al azar solo dentro de la lista asociada
            Node randomNode = patrolNodes[Random.Range(0, patrolNodes.Count)];

            if (randomNode != null)
            {
                MoveTo(randomNode.transform.position);
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyMovement] El objeto {gameObject.name} no tiene nodos asignados en su lista 'Patrol Nodes'.");
        }
    }
}