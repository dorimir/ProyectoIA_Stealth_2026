using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    // Método principal que calcula la ruta y devuelve una lista de posiciones (Vector3)
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = GetClosestNode(startPos);
        Node targetNode = GetClosestNode(targetPos);

        if (startNode == null || targetNode == null) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // Inicializamos los costes del nodo de inicio
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;

        while (openSet.Count > 0)
        {
            // Buscamos el nodo en el openSet con menor fCost
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Si hemos llegado al nodo de destino, reconstruimos el camino
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Recorremos los vecinos del nodo actual
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (neighbor == null || closedSet.Contains(neighbor)) continue;

                int movementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (movementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = movementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode); // Heurística (Distancia estimada)
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // Si no hay camino posible
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.transform.position);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Invertimos para que vaya del inicio al final
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        // Distancia euclidiana multiplicada por 10 para trabajar con enteros (evita decimales pesados)
        return Mathf.RoundToInt(Vector3.Distance(nodeA.transform.position, nodeB.transform.position) * 10f);
    }

    // Encuentra el nodo del grafo más cercano a una posición del mundo (ej. la posición del enemigo)
    public Node GetClosestNode(Vector3 worldPos)
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        Node closestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in allNodes)
        {
            float distance = Vector3.Distance(worldPos, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
}