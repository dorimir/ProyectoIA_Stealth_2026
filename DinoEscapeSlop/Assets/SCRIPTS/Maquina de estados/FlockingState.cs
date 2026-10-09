using System.Collections.Generic;
using UnityEngine;

public class Flockingstate : IState
{
    private EnemyAI ai;

    public Flockingstate(EnemyAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        Debug.Log($"{ai.gameObject.name} entra en FLOCKING (¿Es Líder?: {ai.isLeader})");

        if (ai.Movement.HasReachedDestination)
        {
            EvaluateAndMove();
        }
    }

    public void Update()
    {
        if (ai.Movement.HasReachedDestination)
        {
            EvaluateAndMove();
        }
    }

    public void Exit()
    {
        // Limpieza opcional
    }

    private void EvaluateAndMove()
    {
        // --- CASO 1: EL ENEMIGO ES EL LÍDER ---
        if (ai.isLeader)
        {
            // El líder patrulla libremente por su lista de nodos
            ai.Movement.GoToRandomNode();
            return;
        }

        // --- CASO 2: EL ENEMIGO ES UN SEGUIDOR ---
        Node nextNode = ChooseFollowerNode();

        if (nextNode != null)
        {
            ai.Movement.MoveTo(nextNode.transform.position);
        }
        else
        {
            // Si pierde al líder o no hay nodo válido, patrulla por su cuenta
            ai.Movement.GoToRandomNode();
        }
    }

    private Node ChooseFollowerNode()
    {
        if (ai.Pathfinding == null || ai.leaderTransform == null) return null;

        Node currentNode = ai.Pathfinding.GetClosestNode(ai.transform.position);
        if (currentNode == null || currentNode.neighbors == null || currentNode.neighbors.Count == 0)
            return null;

        // Buscar a otros seguidores cercanos para aplicar SEPARACIÓN
        List<Transform> nearbyAllies = GetNearbyAlliesByTag("Enemy", ai.FlockRadius);

        Node bestNode = null;
        float highestScore = float.MinValue;

        foreach (Node neighborNode in currentNode.neighbors)
        {
            if (neighborNode == null) continue;

            float score = 0f;

            // 1. ATRACCIÓN AL LÍDER (A menor distancia del líder, mayor puntuación)
            float distToLeader = Vector3.Distance(neighborNode.transform.position, ai.leaderTransform.position);
            score -= distToLeader * ai.LeaderFollowWeight;

            // 2. SEPARACIÓN DE OTROS SEGUIDORES (Evitar amontonamientos)
            foreach (var ally in nearbyAllies)
            {
                // Ignoramos al propio líder si está en la lista de aliados para no repelerlo
                if (ally == ai.leaderTransform) continue;

                float distToAlly = Vector3.Distance(neighborNode.transform.position, ally.position);
                if (distToAlly < ai.TooCloseRadius)
                {
                    // Penalizamos fuertemente si el nodo está demasiado cerca de un compañero
                    score -= (ai.TooCloseRadius - distToAlly) * ai.SeparationWeight;
                }
            }

            if (score > highestScore)
            {
                highestScore = score;
                bestNode = neighborNode;
            }
        }

        return bestNode;
    }

    private List<Transform> GetNearbyAlliesByTag(string tag, float radius)
    {
        List<Transform> alliesFound = new List<Transform>();
        Collider[] hitColliders = Physics.OverlapSphere(ai.transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(tag) && hitCollider.gameObject != ai.gameObject)
            {
                alliesFound.Add(hitCollider.transform);
            }
        }

        return alliesFound;
    }
}