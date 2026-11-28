using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
public sealed class BombProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float lifetime = 15f;

    private Transform owner;
    private float stunDuration;
    private bool hasHit;

    public void Initialize(Transform player, float duration)
    {
        owner = player;
        stunDuration = duration;

        // Spawn in place. The sphere only detects guards; it never pushes objects.
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.enabled = true;
        trigger.isTrigger = true;
        foreach (Collider otherCollider in GetComponentsInChildren<Collider>(true))
        {
            if (otherCollider != trigger)
                otherCollider.enabled = false;
        }

        // A user-assigned visual prefab may already include a Rigidbody.
        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
        {
            body.isKinematic = true;
            body.useGravity = false;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (hasHit || owner == null || other.transform.IsChildOf(owner))
            return;

        EnemyPatrol guard = other.GetComponentInParent<EnemyPatrol>();
        if (guard == null)
            return;

        hasHit = true;
        guard.Stun(stunDuration);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
