using UnityEngine;

public class RastreoSonoro : MonoBehaviour
{
    public Collider Area;
    public GameObject Player;
    public AudioSource sonido;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador ha entrado en la zona");
            sonido.Play();
        }
    }
}