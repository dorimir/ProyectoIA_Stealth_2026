using UnityEngine;

public class EnemyPatrolAI : MonoBehaviour
{
    public enum ModoPatrulla { Circular, IdaYVuelta }

    [Header("Patrulla")]
    public Transform[] waypoints;
    public ModoPatrulla modo = ModoPatrulla.Circular;
    public float velocidad = 3f;
    [Tooltip("Distancia a la que se considera que ha llegado al waypoint.")]
    public float distanciaLlegada = 0.3f;
    [Tooltip("Segundos que espera en cada waypoint (0 = no se detiene).")]
    public float tiempoEspera = 1.5f;

    [Header("Rotación")]
    [Tooltip("Más alto = gira antes hacia su destino.")]
    public float rotationSmoothness = 8f;

    public StateMachine Machine { get; private set; }
    public PatrolState Patrol { get; private set; }
    public WaitState Wait { get; private set; }

    private int indice = 0;
    private int direccion = 1;

    void Start()
    {
        Machine = new StateMachine();
        Patrol = new PatrolState(this);
        Wait = new WaitState(this);

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: te falta los waypoints manin :).", this);
            enabled = false;
            return;
        }

        indice = WaypointMasCercano();
        Machine.Initialize(Patrol);
    }

    void Update()
    {
        Machine.Update();
    }


    public Transform WaypointActual => waypoints[indice];

    public bool MoverHaciaWaypoint()
    {
        Vector3 objetivo = WaypointActual.position;
        objetivo.y = transform.position.y; 

        Vector3 dir = objetivo - transform.position;

        if (dir.magnitude <= distanciaLlegada)
            return true;

        transform.position = Vector3.MoveTowards(
            transform.position, objetivo, velocidad * Time.deltaTime);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            float k = 1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, k);
        }

        return false;
    }

    public void SiguienteWaypoint()
    {
        if (waypoints.Length <= 1) return;

        if (modo == ModoPatrulla.Circular)
        {
            indice = (indice + 1) % waypoints.Length;
        }
        else
        {
            if (indice + direccion < 0 || indice + direccion >= waypoints.Length)
                direccion *= -1;
            indice += direccion;
        }
    }

    public void CambiarEstado(IState nuevo) => Machine.ChangeState(nuevo);

    int WaypointMasCercano()
    {
        int mejor = 0;
        float mejorDist = float.PositiveInfinity;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float d = (waypoints[i].position - transform.position).sqrMagnitude;
            if (d < mejorDist) { mejorDist = d; mejor = i; }
        }
        return mejor;
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            bool hayNext = i < waypoints.Length - 1;
            if (hayNext && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            else if (!hayNext && modo == ModoPatrulla.Circular && waypoints[0] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
        }
    }
}