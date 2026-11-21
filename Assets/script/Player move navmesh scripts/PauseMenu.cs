using UnityEngine;

[DisallowMultipleComponent]
public sealed class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TapToMove playerInput;

    private float previousTimeScale;
    private bool wasInputEnabled;

    public void Pause()
    {
        if (pausePanel.activeSelf || gameOverPanel.activeSelf || Time.timeScale <= 0f)
            return;

        previousTimeScale = Time.timeScale;
        wasInputEnabled = playerInput.enabled;
        playerInput.enabled = false;

        Time.timeScale = 0f;
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!pausePanel.activeSelf || gameOverPanel.activeSelf)
            return;

        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        playerInput.enabled = wasInputEnabled;
        Time.timeScale = previousTimeScale;
    }
}
