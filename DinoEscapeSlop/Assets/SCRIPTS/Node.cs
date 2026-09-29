using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Conexiones")]
    [Tooltip("Lista de nodos a los que se puede ir directamente desde este nodo.")]
    public List<Node> neighbors = new List<Node>();

    // Variables internas para el algoritmo A* (no hace falta tocarlas en el inspector)
    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    [HideInInspector] public int fCost => gCost + hCost;
    [HideInInspector] public Node parent;

    // Dibuja líneas en el editor para ver las conexiones entre nodos fácilmente
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (neighbors != null)
        {
            Gizmos.color = Color.green;
            foreach (Node neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(transform.position, neighbor.transform.position);
                }
            }
        }
    }
}