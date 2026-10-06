public class PatrolState : IState
{
    private readonly EnemyPatrolAI ai;

    public PatrolState(EnemyPatrolAI ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        
    }

    public void Update()
    {

        if (ai.MoverHaciaWaypoint())
        {
            ai.SiguienteWaypoint();

            if (ai.tiempoEspera > 0f)
                ai.CambiarEstado(ai.Wait);
        }
    }

    public void Exit() { }
}
