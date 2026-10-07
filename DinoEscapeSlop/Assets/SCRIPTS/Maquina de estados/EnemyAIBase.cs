using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    public EnemyMovement Movement { get; private set; }
    public AStarPathfinding Pathfinding { get; private set; }

    [Header("Rol en el Flocking")]
    [Tooltip("Marca esta casilla solo en el enemigo que actuará como Líder de la manada.")]
    public bool isLeader = false;

    [Tooltip("Si NO es líder, asigna aquí al enemigo líder al que debe seguir.")]
    public Transform leaderTransform;

    [Header("Configuración de Flocking (Seguidores)")]
    [Tooltip("Radio de detección para evitar colisiones con otros seguidores.")]
    public float FlockRadius = 10f;

    [Tooltip("Distancia mínima de separación con otros enemigos.")]
    public float TooCloseRadius = 2.5f;

    [Header("Pesos del Flocking")]
    [Tooltip("Fuerza con la que huye de otros seguidores si se acercan demasiado.")]
    public float SeparationWeight = 4f;

    [Tooltip("Fuerza con la que intenta acercarse al Líder.")]
    public float LeaderFollowWeight = 3f;

    // --- MÁQUINA DE ESTADOS Y ESTADOS ---
    public StateMachine StateMachine { get; private set; }
    public Flockingstate FlockingState { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Pathfinding = GetComponent<AStarPathfinding>();

        if (Pathfinding == null)
        {
            Pathfinding = FindObjectOfType<AStarPathfinding>();
        }

        StateMachine = new StateMachine();
        FlockingState = new Flockingstate(this);
    }

    private void Start()
    {
        StateMachine.Initialize(FlockingState);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isLeader ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, FlockRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TooCloseRadius);
    }
}