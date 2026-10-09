using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    public EnemyMovement Movement { get; private set; }
    public AStarPathfinding Pathfinding { get; private set; }

    [Header("Jugador")]
    public Transform Player;

    [Header("Configuración de Rol y Líder (FLOCKING)")]
    [Tooltip("¿Este enemigo es el líder del grupo?")]
    public bool isLeader = false;

    [Tooltip("Transform del enemigo marcado como Líder (para los seguidores)")]
    public Transform leaderTransform;

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
    [Tooltip("Prioridad para seguir al líder")]
    public float LeaderFollowWeight = 3f;

    [Tooltip("Prioridad para no amontonarse con compañeros")]
    public float SeparationWeight = 1.5f;

    public float CohesionWeight = 1.5f;
    public float AlignmentWeight = 1f;

    // Máquina y estados
    public StateMachine StateMachine { get; private set; }
    public Flockingstate FlockingState { get; private set; }
    public ChaseState ChaseState { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Pathfinding = FindObjectOfType<AStarPathfinding>();

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

    /// <summary>
    /// Gizmos para depuración del Flocking
    /// </summary>
    private void OnDrawGizmos()
    {
        if (isLeader)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.4f);
        }
        else if (leaderTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, leaderTransform.position + Vector3.up * 0.5f);
        }

        // Radio de separación
        Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, TooCloseRadius);

        // Radio de percepción de aliados
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, FlockRadius);
    }
}