using UnityEngine;

public class Dino : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public float velocidad = 2f;
    public float rangoPersecucion = 5f;
    public float rangoAtaque = 2f;
    public int vida = 50;

    private StateMachine fsm;

    void Start()
    {
        fsm = new StateMachine();
        // Iniciamos en Patrullar
        fsm.Initialize(new PatrullarState(this, fsm));
    }

    void Update()
    {
        fsm.Update();
    }

}