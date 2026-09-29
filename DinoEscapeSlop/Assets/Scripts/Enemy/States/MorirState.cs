using UnityEngine;

public class MorirState : MonoBehaviour, IState
{
    private readonly Enemigo enemigo;
    private readonly StateMachine fsm;

    public MorirState(Enemigo enemigo, StateMachine fsm)
    {
        this.enemigo = enemigo;
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("Estado: Morir");
        // Aquí podrías reproducir animación de muerte
        Object.Destroy(enemigo.gameObject, 1f);  // Destruye tras 1s
    }

    public void Execute() { /* No hay lógica continua */ }

    public void Exit() { /* Nunca saldrá de morir */ }
}