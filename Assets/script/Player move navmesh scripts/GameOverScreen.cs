using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private bool isGameOver;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckHazard(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckHazard(other);
    }

    private void CheckHazard(Collider other)
    {
        EnemyPatrol guard = other.GetComponentInParent<EnemyPatrol>();
        if (guard != null && guard.IsStunned)
            return;

        if (other.CompareTag("Obstacle") || guard != null)
            ShowGameOver();
    }

    public void ShowGameOver()
    {
        if (isGameOver || panel == null)
            return;

        isGameOver = true;
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
}
