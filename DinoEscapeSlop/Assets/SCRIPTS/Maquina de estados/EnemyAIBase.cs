using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    public EnemyMovement Movement { get; private set; }
    public AStarPathfinding Pathfinding { get; private set; }

    [Header("Configuración de Flocking")]
    [Tooltip("Radio en el que busca aliados cerca")]
    public float FlockRadius = 10f;

    [Tooltip("Distancia mínima para empezar a separarse de los aliados")]
    public float TooCloseRadius = 2.5f;

    [Header("Pesos del Flocking")]
    public float SeparationWeight = 3f;
    public float CohesionWeight = 1.5f;
    public float AlignmentWeight = 1f;

    private void Awake()
    {
        // Se obtienen las referencias de las componentes en el mismo GameObject
        Movement = GetComponent<EnemyMovement>();
        Pathfinding = GetComponent<AStarPathfinding>();
    }
}