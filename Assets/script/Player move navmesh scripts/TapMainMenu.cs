using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class TapMainMenu : MonoBehaviour
{
    [Tooltip("Scene names or build paths for Levels 1–10. Empty slots show Coming Soon.")]
    [SerializeField] private string[] levelScenes = new string[10];
    [SerializeField] private UnityEngine.UI.Button[] levelButtons = new UnityEngine.UI.Button[10];
    [SerializeField] private TMP_Text[] levelStatusLabels = new TMP_Text[10];
    [SerializeField] private GameObject[] levelLockIcons = new GameObject[10];
    [SerializeField] private Color availableLevelColor = new Color32(83, 116, 58, 255);
    [SerializeField] private Color lockedLevelColor = new Color32(235, 225, 196, 255);
    [SerializeField] private Color availableTextColor = new Color32(255, 246, 216, 255);
    [SerializeField] private Color lockedTextColor = new Color32(77, 73, 49, 255);
    [SerializeField] private UnityEngine.UI.Button playButton;
    [SerializeField] private UnityEngine.UI.Button quitButton;
    [SerializeField] private TMP_Text availabilityLabel;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private RectTransform safeArea;
    [SerializeField] private RectTransform headerBoard;
    [SerializeField] private UnityEngine.UI.GridLayoutGroup levelGrid;
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;
    private Vector2 lastCanvasSize;
    private Rect lastSafeArea;
    private bool loading;

    private void Awake()
    {
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
        RefreshLevels();
        RefreshLayout();
    }

    private void Update()
    {
        if (lastCanvasSize != ((RectTransform)transform).rect.size || lastSafeArea != Screen.safeArea)
            RefreshLayout();
    }

    public void RefreshLevels()
    {
        int readyCount = 0;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool ready = IsLevelReady(i);
            levelButtons[i].interactable = ready && !loading;
            levelStatusLabels[i].text = ready ? "PLAY" : "COMING SOON";
            if (i < levelLockIcons.Length && levelLockIcons[i] != null)
                levelLockIcons[i].SetActive(!ready);

            Color faceColor = ready ? availableLevelColor : lockedLevelColor;
            UnityEngine.UI.ColorBlock colors = levelButtons[i].colors;
            colors.normalColor = colors.disabledColor = faceColor;
            colors.highlightedColor = Color.Lerp(faceColor, ready ? Color.black : Color.white, 0.12f);
            colors.selectedColor = Color.Lerp(faceColor, ready ? Color.black : Color.white, 0.08f);
            colors.pressedColor = Color.Lerp(faceColor, Color.black, 0.2f);
            levelButtons[i].colors = colors;
            foreach (TMP_Text label in levelButtons[i].GetComponentsInChildren<TMP_Text>(true))
                label.color = ready ? availableTextColor : lockedTextColor;
            if (ready)
                readyCount++;
        }

        playButton.interactable = readyCount > 0 && !loading;
        availabilityLabel.text = readyCount + " / " + levelButtons.Length + " UNLOCKED";
        messageLabel.text = readyCount < levelButtons.Length
            ? "More levels are coming soon."
            : "Tap a level to begin.";
    }

    public void RefreshLayout()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect bounds = Screen.safeArea;
        safeArea.anchorMin = new Vector2(bounds.xMin / Screen.width, bounds.yMin / Screen.height);
        safeArea.anchorMax = new Vector2(bounds.xMax / Screen.width, bounds.yMax / Screen.height);
        safeArea.offsetMin = safeArea.offsetMax = Vector2.zero;
        Canvas.ForceUpdateCanvases();

        if (headerBoard != null)
            headerBoard.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Mathf.Min(safeArea.rect.width - 80f, 850f));

        int columns = (float)Screen.width / Screen.height < 1.2f ? 2 : 5;
        int rows = Mathf.CeilToInt((float)levelButtons.Length / columns);
        Vector2 size = ((RectTransform)levelGrid.transform).rect.size;
        levelGrid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
        levelGrid.constraintCount = columns;
        levelGrid.cellSize = new Vector2(
            (size.x - levelGrid.padding.horizontal - levelGrid.spacing.x * (columns - 1)) / columns,
            (size.y - levelGrid.padding.vertical - levelGrid.spacing.y * (rows - 1)) / rows);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)levelGrid.transform);

        lastCanvasSize = ((RectTransform)transform).rect.size;
        lastSafeArea = bounds;
    }

    public void LoadLevel(int levelIndex)
    {
        if (loading || !IsLevelReady(levelIndex))
            return;

        PlayClick();
        loading = true;
        foreach (UnityEngine.UI.Button button in levelButtons)
            button.interactable = false;
        playButton.interactable = false;
        quitButton.interactable = false;
        messageLabel.text = "Loading Level " + (levelIndex + 1) + "...";
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(levelScenes[levelIndex], LoadSceneMode.Single);
    }

    public void PlayFirstLevel()
    {
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if (!IsLevelReady(i))
                continue;

            LoadLevel(i);
            return;
        }
    }

    public void QuitGame()
    {
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool IsLevelReady(int index)
    {
        if (index < 0 || index >= levelScenes.Length || string.IsNullOrWhiteSpace(levelScenes[index]))
            return false;

#if UNITY_EDITOR
        // The runtime streaming check can return false in an Edit Mode preview.
        if (!Application.isPlaying)
        {
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (scene.enabled && (scene.path == levelScenes[index] ||
                    System.IO.Path.GetFileNameWithoutExtension(scene.path) == levelScenes[index]))
                    return true;
            }
            return false;
        }
#endif
        return Application.CanStreamedLevelBeLoaded(levelScenes[index]);
    }

    private void PlayClick()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }

}
