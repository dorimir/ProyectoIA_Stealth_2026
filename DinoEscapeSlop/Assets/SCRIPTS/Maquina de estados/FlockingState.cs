using System.Collections.Generic;
using UnityEngine;

public class Flockingstate : IState
{
    private EnemyAI ai;

    // Constructor que recibe el controlador principal EnemyAI
    public Flockingstate(EnemyAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        Debug.Log($"{ai.gameObject.name} ha entrado en estado: FLOCKING");
    }

    public void Update()
    {
        
        if (ai.Movement.HasReachedDestination)
        {
            Node nextNode = ChooseNextFlockNode();

            if (nextNode != null)
            {
                ai.Movement.MoveTo(nextNode.transform.position);
            }
            else
            {
                ai.Movement.GoToRandomNode();
            }
        }
    }

    public void Exit()
    {
        // Limpieza opcional al salir del estado de Flocking
    }



    private Node ChooseNextFlockNode()
    {
        if (ai.Pathfinding == null) return null;

        Node currentNode = ai.Pathfinding.GetClosestNode(ai.transform.position);
        if (currentNode == null || currentNode.neighbors == null || currentNode.neighbors.Count == 0)
            return null;

        // --- AHORA BUSCAMOS ALlADOS POR ETIQUETA ---
        List<Transform> allies = GetNearbyAlliesByTag("Enemy", ai.FlockRadius);
        if (allies.Count == 0) return null; 

        Vector3 centerOfMass = Vector3.zero;
        Vector3 averageForward = Vector3.zero;

        foreach (var ally in allies)
        {
            centerOfMass += ally.position;
            averageForward += ally.forward;
        }

        centerOfMass /= allies.Count;
        averageForward = averageForward.normalized;

        Node bestNode = null;
        float highestScore = float.MinValue;

        foreach (Node neighborNode in currentNode.neighbors)
        {
            if (neighborNode == null) continue;

            float score = 0f;

            // 1. SEPARACIÓN
            foreach (var ally in allies)
            {
                float distToAlly = Vector3.Distance(neighborNode.transform.position, ally.position);
                if (distToAlly < ai.TooCloseRadius)
                {
                    score -= (ai.TooCloseRadius - distToAlly) * ai.SeparationWeight;
                }
            }

            // 2. COHESIÓN
            float distToCenter = Vector3.Distance(neighborNode.transform.position, centerOfMass);
            score -= distToCenter * ai.CohesionWeight;

            // 3. ALINEACIÓN
            Vector3 directionToNode = (neighborNode.transform.position - currentNode.transform.position).normalized;
            float alignmentDot = Vector3.Dot(directionToNode, averageForward);
            score += alignmentDot * ai.AlignmentWeight;

            if (score > highestScore)
            {
                highestScore = score;
                bestNode = neighborNode;
            }
        }

        return bestNode;
    }

    // --- MÉTODO PARA BUSCAR ALIADOS POR TAG EN UN RADIO ---
    private List<Transform> GetNearbyAlliesByTag(string tag, float radius)
    {
        List<Transform> alliesFound = new List<Transform>();

        // Busca todas las colisiones físicas en la esfera de percepción
        Collider[] hitColliders = Physics.OverlapSphere(ai.transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            // Comprobar que sea un enemigo Y que no se detecte a sí mismo
            if (hitCollider.CompareTag(tag) && hitCollider.gameObject != ai.gameObject)
            {
                alliesFound.Add(hitCollider.transform);
            }
        }

        return alliesFound;
    }
}