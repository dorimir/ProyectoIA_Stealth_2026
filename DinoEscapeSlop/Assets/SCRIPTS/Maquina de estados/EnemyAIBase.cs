using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    public EnemyMovement Movement { get; private set; }
    public AStarPathfinding Pathfinding { get; private set; }

    [Header("Jugador")]
    public Transform Player;

    [Header("Configuración de Flee")]
    public bool IsPrey = false;        // true = huye del jugador, false = lo persigue
    public float SafeRange = 22f;      // distancia a la que deja de huir

    public FleeState FleeState { get; private set; }

    [Header("Configuración de Chase")]
    public float DetectionRange = 15f;
    public float LoseRange = 25f;
    public float AttackRange = 2.5f;

    [Header("Configuración de Flocking")]
    [Tooltip("Radio en el que busca aliados cerca")]
    public float FlockRadius = 10f;

    [Tooltip("Distancia mínima para empezar a separarse de los aliados")]
    public float TooCloseRadius = 2.5f;

    [Header("Pesos del Flocking")]
    public float SeparationWeight = 3f;
    public float CohesionWeight = 1.5f;
    public float AlignmentWeight = 1f;

    // Máquina y estados
    public StateMachine StateMachine { get; private set; }
    public Flockingstate FlockingState { get; private set; }
    public ChaseState ChaseState { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Pathfinding = GetComponent<AStarPathfinding>();

        StateMachine = new StateMachine();
        FlockingState = new Flockingstate(this);
        ChaseState = new ChaseState(this);
        FleeState = new FleeState(this);
    }

    private void Start()
    {
        if (Player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Player = p.transform;
        }

        StateMachine.Initialize(FlockingState);
    }

    private void Update()
    {
        StateMachine.Update();
    }
}