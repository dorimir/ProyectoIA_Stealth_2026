using UnityEngine;

public class FleeState : IState
{
    private EnemyAI ai;
    private Node previousNode;
    private float reevaluateTimer;

    private const float ReevaluateInterval = 0.5f;
    private const float BacktrackPenalty = 5f;

    public FleeState(EnemyAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        Debug.Log($"{ai.gameObject.name} ha entrado en estado: FLEE");
        previousNode = null;
        reevaluateTimer = 0f; // elige ruta de huida inmediatamente
    }

    public void Update()
    {
        if (ai.Player == null)
        {
            ai.StateMachine.ChangeState(ai.FlockingState);
            return;
        }

        float distance = Vector3.Distance(ai.transform.position, ai.Player.position);

        // Ya está a salvo: vuelve a la manada
        if (distance > ai.SafeRange)
        {
            ai.StateMachine.ChangeState(ai.FlockingState);
            return;
        }

        reevaluateTimer -= Time.deltaTime;

        // Elige nuevo nodo al llegar al destino o cada cierto tiempo
        // (el jugador se mueve, así que la mejor dirección cambia)
        if (ai.Movement.HasReachedDestination || reevaluateTimer <= 0f)
        {
            Node next = ChooseEscapeNode();

            if (next != null)
                ai.Movement.MoveTo(next.transform.position);
            else
                ai.Movement.GoToRandomNode();

            reevaluateTimer = ReevaluateInterval;
        }
    }

    public void Exit() { }

    private Node ChooseEscapeNode()
    {
        if (ai.Pathfinding == null) return null;

        Node currentNode = ai.Pathfinding.GetClosestNode(ai.transform.position);
        if (currentNode == null || currentNode.neighbors == null || currentNode.neighbors.Count == 0)
            return null;

        Node bestNode = null;
        float highestScore = float.MinValue;

        foreach (Node neighbor in currentNode.neighbors)
        {
            if (neighbor == null) continue;

            // Cuanto más lejos del jugador, mejor
            float score = Vector3.Distance(neighbor.transform.position, ai.Player.position);

            // Evita ir y volver entre dos nodos
            if (neighbor == previousNode)
                score -= BacktrackPenalty;

            if (score > highestScore)
            {
                highestScore = score;
                bestNode = neighbor;
            }
        }

        previousNode = currentNode;
        return bestNode;
    }
}