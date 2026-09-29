using UnityEngine;

// --- PATRULLA ---
public class PatrolState : IState
{
    private EnemyAI ai;

    public PatrolState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: PATRULLA"); }

    public void Update()
    {
        // Lógica de movimiento entre puntos de patrulla...

        // Transición de ejemplo: Detecta al jugador
        if (ai.CanSeePlayer())
        {
            ai.StateMachine.ChangeState(ai.ChaseState);
        }
    }

    public void Exit() { }
}

// --- PERSECUCIÓN ---
public class ChaseState : IState
{
    private EnemyAI ai;

    public ChaseState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: PERSECUCIÓN"); }

    public void Update()
    {
        // Moverse hacia el jugador...

        // Transiciones
        if (ai.IsPlayerInAttackRange())
        {
            ai.StateMachine.ChangeState(ai.AttackState);
        }
        else if (!ai.CanSeePlayer())
        {
            // Pierde la vista direct: pasa a rastreo
            ai.StateMachine.ChangeState(ai.TrackState);
        }
        else if (ai.Health < ai.LowHealthThreshold)
        {
            ai.StateMachine.ChangeState(ai.FleeState);
        }
    }

    public void Exit() { }
}

// --- ATAQUE ---
public class AttackState : IState
{
    private EnemyAI ai;

    public AttackState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: ATAQUE"); }

    public void Update()
    {
        // Realizar ataques / animaciones...

        if (!ai.IsPlayerInAttackRange())
        {
            ai.StateMachine.ChangeState(ai.ChaseState);
        }
    }

    public void Exit() { }
}

// --- HUIDA ---
public class FleeState : IState
{
    private EnemyAI ai;

    public FleeState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: HUIDA"); }

    public void Update()
    {
        // Moverse en dirección opuesta al jugador/amenaza...
    }

    public void Exit() { }
}

// --- RASTREO ---
public class TrackState : IState
{
    private EnemyAI ai;

    public TrackState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: RASTREO"); }

    public void Update()
    {
        // Investigar la última posición conocida del jugador...

        if (ai.CanSeePlayer())
        {
            ai.StateMachine.ChangeState(ai.ChaseState);
        }
        // Si pasa el tiempo y no encuentra nada -> Patrulla o Flocking
    }

    public void Exit() { }
}

// --- FLOCKING (Comportamiento de Manada) ---
public class FlockingState : IState
{
    private EnemyAI ai;

    public FlockingState(EnemyAI ai) { this.ai = ai; }

    public void Enter() { Debug.Log("Entrando en estado: FLOCKING"); }

    public void Update()
    {
        // Aplicar reglas de Alineación, Cohesión y Separación con el grupo...

        if (ai.CanSeePlayer())
        {
            ai.StateMachine.ChangeState(ai.ChaseState);
        }
    }

    public void Exit() { }
}