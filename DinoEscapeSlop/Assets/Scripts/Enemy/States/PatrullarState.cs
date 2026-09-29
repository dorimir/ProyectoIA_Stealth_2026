using UnityEngine;

public class PatrullaState : MonoBehaviour, IState
{
    private readonly Enemigo enemigo;
    private readonly StateMachine fsm;
    private Vector3 puntosDestino;

    public PatrullaState(Enemigo enemigo, StateMachine fsm)
    {
        this.enemigo = enemigo;
        this.fsm = fsm;
    }

    public void Enter()
    {
        puntosDestino = enemigo.transform.position + Random.insideUnitSphere * 4f;
        puntosDestino.y = enemigo.transform.position.y;
        Debug.Log("Patrullando");
    }

    public void Execute()
    {
        enemigo.transform.position = Vector3.MoveTowards(
            enemigo.transform.position, puntosDestino,
            enemigo.velocidad * Time.deltaTime);

        if(Vector3.Distance(enemigo.transform.position, puntosDestino) < 0.1f)
        {
            Enter();
        }

        float dist = Vector3.Distance(enemigo.transform.position, enemigo.jugador.position);

        if(dist <= enemigo.rangoPersecucion)
        {
            fsm.ChangeState(new PerseguirState(enemigo, fsm));
        }
    }

    public void Exit()
    {
        Debug.Log("Saliendo de Patrullar");
    }
}