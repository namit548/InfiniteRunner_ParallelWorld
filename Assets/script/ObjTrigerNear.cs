using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

[MovedFrom(true, null, "Assembly-CSharp", "ObjTriggerEnter")]
public class ObjTrigerNear : MonoBehaviour
{
    public enum DetectionMode
    {
        Radius,
        Collider
    }

    [HideInInspector] public string objName;
    [HideInInspector] public bool objEntered;
    [HideInInspector] public bool colliderCheck;

    [Tooltip("The colliders allowed to activate this detector, for example the player's collider.")]
    public List<Collider> enterColliders = new List<Collider>();

    [Min(0f)] public float distanceRadius = 2f;
    [Tooltip("Optional center for radius detection. Leave empty to use this object's position.")]
    public Transform detectionOrigin;

    [HideInInspector] public float timeInSecond = 0.7f;
    public UnityEvent ObjEnteredEvt = new UnityEvent();
    public UnityEvent ObjExitedEvt = new UnityEvent();

    public bool isObjectInside { get; private set; }

    // Keep the existing serialized checkbox so current scene settings survive.
    public DetectionMode Mode
    {
        get => colliderCheck ? DetectionMode.Collider : DetectionMode.Radius;
        set => colliderCheck = value == DetectionMode.Collider;
    }

    private readonly HashSet<Collider> insideColliders = new HashSet<Collider>();
    private readonly List<Collider> invalidColliders = new List<Collider>();
    private bool dragObject;

    private void OnEnable()
    {
        ResetVariables();
    }

    private void OnDisable()
    {
        ResetVariables();
    }

    private void FixedUpdate()
    {
        if (!colliderCheck)
        {
            insideColliders.Clear();
            SetDetected(IsNearAnyCollider());
            return;
        }

        // Disabled or destroyed targets do not always send OnTriggerExit.
        invalidColliders.Clear();
        foreach (Collider collider in insideColliders)
        {
            if (!IsAcceptedCollider(collider))
                invalidColliders.Add(collider);
        }
        foreach (Collider collider in invalidColliders)
            insideColliders.Remove(collider);

        SetDetected(insideColliders.Count > 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        TrackCollider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Detect an existing overlap after enabling the script or changing modes.
        TrackCollider(other);
    }

    private void TrackCollider(Collider other)
    {
        if (!isActiveAndEnabled || !colliderCheck || !IsAcceptedCollider(other))
            return;

        insideColliders.Add(other);
        SetDetected(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActiveAndEnabled || !colliderCheck)
            return;

        insideColliders.Remove(other);
        SetDetected(insideColliders.Count > 0);
    }

    private bool IsAcceptedCollider(Collider other)
    {
        return other != null && other.enabled && other.gameObject.activeInHierarchy &&
            enterColliders != null && enterColliders.Contains(other);
    }

    private bool IsNearAnyCollider()
    {
        if (enterColliders == null)
            return false;

        Vector3 center = detectionOrigin != null ? detectionOrigin.position : transform.position;
        float radius = Mathf.Max(0f, distanceRadius);
        foreach (Collider target in enterColliders)
        {
            if (!IsAcceptedCollider(target))
                continue;

            Vector3 closest = target.ClosestPoint(center);
            if ((closest - center).sqrMagnitude <= radius * radius)
                return true;
        }
        return false;
    }

    private void SetDetected(bool detected)
    {
        isObjectInside = detected;
        bool entered = detected || (dragObject && objEntered);
        if (entered == objEntered)
            return;

        objEntered = entered;
        if (entered)
            ObjEnteredEvt?.Invoke();
        else
            ObjExitedEvt?.Invoke();
    }

    public void ObjectPointerEnableDisableDelay(bool value, bool dragvalue)
    {
        if (!objEntered)
            gameObject.SetActive(value);
        dragObject = dragvalue;
    }

    public void ResetVariables()
    {
        insideColliders.Clear();
        invalidColliders.Clear();
        objEntered = false;
        isObjectInside = false;
        dragObject = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (colliderCheck)
            return;

        Gizmos.color = objEntered ? Color.green : Color.yellow;
        Vector3 center = detectionOrigin != null ? detectionOrigin.position : transform.position;
        Gizmos.DrawWireSphere(center, Mathf.Max(0f, distanceRadius));
    }
}
