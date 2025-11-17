using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManger : MonoBehaviour
{
   // public TextMeshProUGUI scoreText;
    public int score = 0;

    private void Start()
    {
        InvokeRepeating("increaseScore", 1f, 1f);
            }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //public void increaseScore()
    //{
    //    score    += 1;
    //    scoreText.text= score.ToString();
    //}
}
