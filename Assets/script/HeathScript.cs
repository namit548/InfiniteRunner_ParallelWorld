using UnityEngine;
using UnityEngine.UI;

public class HeathScript : MonoBehaviour
{

    public GameObject gameWinPanel;
    public float totalHealth =100f;
    public float EnemyHealth = 100f;
    public Slider EnemyHealthSlider;
    public AudioSource PainSound;
  



    void Start()
    {
          EnemyHealth = totalHealth;
       
    }
    public void DamangePlasyer(float damage)
    {
        EnemyHealth -= damage;
        EnemyHealthSlider.value = EnemyHealth;
        if (EnemyHealthSlider.value == 0)
        {
            Debug.Log("Win the game");
            Time.timeScale = 0f;
            gameWinPanel.SetActive(true);
        }
    }

    public void WinWin()
    {

    }
    //void Update()
    //{

    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        DamangePlasyer(30f);
    //        Debug.Log("Damange");
    //    }

    //}
}
