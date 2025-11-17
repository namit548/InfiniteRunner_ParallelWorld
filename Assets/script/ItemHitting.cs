using UnityEngine;

public class ItemHitting : MonoBehaviour
{
    public float damageAmount = 20f;
  

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
           

            // Get the HeathScript from the enemy
            HeathScript enemyHealth = collision.gameObject.GetComponent<HeathScript>();

            if (enemyHealth != null)
            {
                enemyHealth.DamangePlasyer(damageAmount);
            }

            HeathScript painSound = collision.gameObject.GetComponent<HeathScript>();
            if (painSound != null) {

                enemyHealth.PainSound.Play();
            }

        }
    }
}
