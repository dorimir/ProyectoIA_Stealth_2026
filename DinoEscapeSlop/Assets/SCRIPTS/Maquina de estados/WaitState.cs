using UnityEngine;

public class WaitState : IState
{
    private readonly EnemyPatrolAI ai;
    private float timer;

    public WaitState(EnemyPatrolAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        timer = ai.tiempoEspera;
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
            ai.CambiarEstado(ai.Patrol);
    }

    public void Exit() { }
}