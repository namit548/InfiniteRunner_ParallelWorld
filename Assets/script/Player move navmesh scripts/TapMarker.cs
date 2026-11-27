using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public sealed class TapMarker : MonoBehaviour
{
    private const int SegmentCount = 48;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private LineRenderer ring;
    private MaterialPropertyBlock properties;
    private Color color;
    private float radius;
    private float duration;
    private float elapsed;

    private void Awake()
    {
        ring = GetComponent<LineRenderer>();
        properties = new MaterialPropertyBlock();
        ring.sharedMaterial = Resources.Load<Material>("TapMarker");
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = SegmentCount;
        ring.startWidth = ring.endWidth = 1f;
        ring.shadowCastingMode = ShadowCastingMode.Off;
        ring.receiveShadows = false;

        var points = new Vector3[SegmentCount];
        for (int i = 0; i < SegmentCount; i++)
        {
            float angle = i * Mathf.PI * 2f / SegmentCount;
            points[i] = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        }
        ring.SetPositions(points);

        if (ring.sharedMaterial == null)
            Debug.LogError("TapMarker needs the material at Resources/TapMarker.mat.", this);

        Hide();
    }

    public void Show(Vector3 position, Vector3 normal, Color markerColor, float markerRadius, float lifetime)
    {
        if (ring.sharedMaterial == null)
            return;

        color = markerColor;
        radius = Mathf.Max(0.01f, markerRadius);
        duration = Mathf.Max(0.01f, lifetime);
        elapsed = 0f;

        // Lift the ring slightly so it does not flicker against the ground.
        transform.SetPositionAndRotation(position + normal * 0.04f,
            Quaternion.FromToRotation(Vector3.up, normal));
        ApplyProgress(0f);
        ring.enabled = true;
        enabled = true;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            Hide();
            return;
        }

        ApplyProgress(elapsed / duration);
    }

    private void ApplyProgress(float progress)
    {
        float fade = 1f - progress;
        float expansion = 1f - fade * fade;
        transform.localScale = Vector3.one * Mathf.Lerp(radius, radius * 1.75f, expansion);
        ring.widthMultiplier = Mathf.Lerp(0.04f, 0.01f, progress);

        Color tint = color;
        tint.a *= fade;
        properties.SetColor(BaseColor, tint);
        ring.SetPropertyBlock(properties);
    }

    public void Hide()
    {
        ring.enabled = false;
        enabled = false;
    }
}
