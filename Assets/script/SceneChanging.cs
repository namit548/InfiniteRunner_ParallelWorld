using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChnaging : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SwitchScenes(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
        
    }
}
