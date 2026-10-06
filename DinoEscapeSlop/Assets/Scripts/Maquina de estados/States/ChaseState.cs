using UnityEngine;

public class ChaseState : IState
{
    private EnemyAI ai;
    private float repathTimer;
    private const float RepathInterval = 0.25f;

    public ChaseState(EnemyAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        Debug.Log($"{ai.gameObject.name} ha entrado en estado: CHASE");
        repathTimer = 0f;
    }

    public void Update()
    {
        if (ai.Player == null)
        {
            ai.StateMachine.ChangeState(ai.FlockingState);
            return;
        }

        float distance = Vector3.Distance(ai.transform.position, ai.Player.position);

        if (distance > ai.LoseRange)
        {
            ai.StateMachine.ChangeState(ai.FlockingState);
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            ai.Movement.MoveTo(ai.Player.position);
            repathTimer = RepathInterval;
        }
    }

    public void Exit() { }
}