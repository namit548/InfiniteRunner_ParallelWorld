using UnityEngine;

[DisallowMultipleComponent]
public sealed class HouseCompletion : MonoBehaviour
{
    [SerializeField] private GameObject congratulationsPanel;

    private void OnEnable()
    {
        if (congratulationsPanel != null)
            congratulationsPanel.SetActive(true);
    }
}
