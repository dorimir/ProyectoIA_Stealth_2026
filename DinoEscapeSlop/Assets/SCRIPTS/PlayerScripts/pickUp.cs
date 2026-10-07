using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public Transform holdPos;

    [Header("Coger / lanzar")]
    public float pickUpRange = 5f;
    public float throwForce = 15f;

    [Header("Suavizado de posición")]
    public float positionSmoothTime = 0.12f;
    [Tooltip("Velocidad máxima con la que puede seguir a la cámara.")]
    public float maxFollowSpeed = 15f;
    [Tooltip("Distancia máxima que puede quedarse por detrás de holdPos (evita que 'vuele' al girar rápido).")]
    public float maxLag = 0.4f;

    [Header("Rotación")]
    [Tooltip("Más alto = sigue antes a la cámara.")]
    public float rotationSmoothness = 8f;
    public float rotationSensitivity = 3f;

    [Header("Soltar")]
    [Tooltip("Separación de la pared/suelo al soltar dentro de la geometría.")]
    public float safeDropMargin = 0.3f;

    private GameObject heldObj;
    private Rigidbody heldObjRb;

    private Transform[] heldTransforms;
    private int[] originalLayers;
    private bool originalKinematic;
    private bool originalDetectCollisions;

    private int holdLayer;
    private Vector3 moveVelocity;
    private Quaternion localRotation; // rotación del objeto relativa a holdPos

    void Start()
    {
        holdLayer = LayerMask.NameToLayer("holdLayer");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
                TryPickUp();
            else
                Release();
        }

        if (heldObj == null)
            return;

        if (Input.GetKey(KeyCode.R))
        {
            float x = Input.GetAxis("Mouse X") * rotationSensitivity;
            float y = Input.GetAxis("Mouse Y") * rotationSensitivity;

            localRotation =
                Quaternion.AngleAxis(-x, Vector3.up) *
                Quaternion.AngleAxis(y, Vector3.right) *
                localRotation;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Throw();
    }

    // LateUpdate: se ejecuta DESPUÉS de que la cámara ya haya girado en Update
    void LateUpdate()
    {
        if (heldObj == null)
            return;

        Transform t = heldObj.transform;

        Vector3 pos = Vector3.SmoothDamp(
            t.position,
            holdPos.position,
            ref moveVelocity,
            positionSmoothTime,
            maxFollowSpeed
        );

        // Nunca se queda más lejos de holdPos que maxLag
        Vector3 offset = pos - holdPos.position;
        if (offset.magnitude > maxLag)
            pos = holdPos.position + offset.normalized * maxLag;

        t.position = pos;

        // Rotación suave
        Quaternion targetRot = holdPos.rotation * localRotation;
        float k = 1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime);
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, k);
    }

    void TryPickUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
        {
            if (hit.transform.CompareTag("canPickUp"))
                PickUp(hit.transform.gameObject);
        }
    }

    void PickUp(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        heldObj = obj;
        heldObjRb = rb;

        // Guardamos estado original
        originalKinematic = rb.isKinematic;
        originalDetectCollisions = rb.detectCollisions;

        heldTransforms = obj.GetComponentsInChildren<Transform>();
        originalLayers = new int[heldTransforms.Length];
        for (int i = 0; i < heldTransforms.Length; i++)
            originalLayers[i] = heldTransforms[i].gameObject.layer;

        rb.linearVelocity = Vector3.zero;       
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        // Capa que solo ve la cámara Overlay
        if (holdLayer >= 0)
            foreach (Transform tr in heldTransforms)
                tr.gameObject.layer = holdLayer;

        localRotation = Quaternion.Inverse(holdPos.rotation) * obj.transform.rotation;
        moveVelocity = Vector3.zero;
    }

    void Release()
    {
        if (heldObj == null)
            return;

        // Como se ve por encima de todo, puede estar "dentro" de una pared o del suelo.
        // Lo colocamos en un punto seguro antes de devolverle la física.
        PlaceSafely();

        // Restaurar capas
        for (int i = 0; i < heldTransforms.Length; i++)
            if (heldTransforms[i] != null)
                heldTransforms[i].gameObject.layer = originalLayers[i];

        heldObjRb.isKinematic = originalKinematic;
        heldObjRb.detectCollisions = originalDetectCollisions;

        heldObj = null;
        heldObjRb = null;
        heldTransforms = null;
        originalLayers = null;
    }

    void Throw()
    {
        Rigidbody rb = heldObjRb;
        Release();

        if (rb != null)
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }

    void PlaceSafely()
    {
        int mask = ~0;
        if (holdLayer >= 0)
            mask = ~(1 << holdLayer);

        Vector3 from = transform.position;
        Vector3 to = heldObj.transform.position;

        RaycastHit hit;
        if (Physics.Linecast(from, to, out hit, mask, QueryTriggerInteraction.Ignore))
        {
            heldObj.transform.position = hit.point + hit.normal * safeDropMargin;
        }
    }
}