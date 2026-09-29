using UnityEngine;

public class AtacarState : MonoBehaviour, IState
{
    private readonly Enemigo enemigo;
    private readonly StateMachine fsm;
    private float timer;
    private readonly float tiempoEntreAtaques = 1.2f;

    public AtacarState(Enemigo enemigo, StateMachine fsm)
    {
        this.enemigo = enemigo;
        this.fsm = fsm;
    }

    public void Enter()
    {
        timer = 0f;
        Debug.Log("Estado: Atacar");
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= tiempoEntreAtaques)
        {
            Disparar();
            timer = 0f;
        }

        float dist = Vector3.Distance(
            enemigo.transform.position, enemigo.jugador.position);
        if (dist > enemigo.rangoAtaque)
            fsm.ChangeState(new PerseguirState(enemigo, fsm));
    }

    public void Exit()
    {
        Debug.Log("Saliendo de Atacar");
    }
}