using UnityEngine;

public class SoundEnemy : MonoBehaviour
{
    public AudioSource PainSound;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("throwingObject"))
        {

            PainSound.enabled = true;
        }
        else {
            PainSound.enabled = false;
        }
    }
}
