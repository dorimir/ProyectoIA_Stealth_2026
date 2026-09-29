using UnityEngine;

public class PerseguirState:  MonoBehaviour, IState
{
    private readonly Enemigo enemigo;
    private readonly StateMachine fsm;

    public PerseguirState(Enemigo enemigo, StateMachine fsm)
    {
        this.enemigo = enemigo;
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("Estado: Perseguir");
    }

    public void Execute()
    {
       
    }

    public void Exit()
    {
        Debug.Log("Saliendo de Perseguir");
    }
}