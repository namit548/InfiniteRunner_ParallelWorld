using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PlayerBombInventory : MonoBehaviour
{
    [SerializeField] private Transform bombSpawner;
    [SerializeField, Tooltip("Drag the bomb GameObject prefab to spawn here. Guard detection is added automatically.")]
    private GameObject bombPrefab;
    [SerializeField] private TMP_Text bombCountText;
    [SerializeField] private Button bombButton;
    [SerializeField] private string pickupTag = "Bomb";
    [SerializeField, Min(0.1f)] private float stunDuration = 60f;

    public int BombCount { get; private set; }

    private void Awake()
    {
        RefreshUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryCollect(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCollect(other);
    }

    private void TryCollect(Collider other)
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f ||
            other.GetComponentInParent<BombProjectile>() != null)
            return;

        // Also accept a collider on a child of a pickup tagged Bomb.
        Transform pickup = other.transform;
        while (pickup != null && !pickup.CompareTag(pickupTag))
            pickup = pickup.parent;

        if (pickup == null || !pickup.gameObject.activeInHierarchy)
            return;

        // Disable immediately so multiple colliders cannot count the same pickup twice.
        pickup.gameObject.SetActive(false);
        Destroy(pickup.gameObject);
        BombCount++;
        RefreshUI();
    }

    public void SpawnBomb()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || BombCount <= 0 ||
            bombSpawner == null || bombPrefab == null)
            return;

        GameObject spawnedBomb = Instantiate(bombPrefab, bombSpawner.position, bombSpawner.rotation);
        BombProjectile bomb = spawnedBomb.GetComponent<BombProjectile>();
        if (bomb == null)
            bomb = spawnedBomb.AddComponent<BombProjectile>();

        bomb.Initialize(transform, stunDuration);
        BombCount--;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (bombCountText != null)
            bombCountText.text = BombCount.ToString();

        if (bombButton != null)
            bombButton.interactable = BombCount > 0;
    }
}
