using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody EnY;
    private int[] EnemyPosition = {-3,0,3};
    public void Start()
    {
        EnY = GetComponent<Rigidbody>();
        InvokeRepeating("EnemmyPositionChange", 1f, 1f);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // Use CompareTag for better performance
        {
         //   Debug.Log("Enemy passed through the obstacle (Trigger)");
            EnY.AddForce(Vector3.up * 8, ForceMode.Impulse);
        }   
    }
    private void EnemmyPositionChange()
    {
        int randomValueEnemy = EnemyPosition[Random.Range(0, EnemyPosition.Length)];
       //// Debug.Log(randomValueEnemy); // Prints either -3, 1, or 3
        transform.position = new Vector3(randomValueEnemy, transform.position.y, transform.position.z);

    }



}
