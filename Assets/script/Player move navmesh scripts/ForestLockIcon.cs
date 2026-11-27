using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Forest Lock Icon")]
[RequireComponent(typeof(CanvasRenderer))]
public sealed class ForestLockIcon : MaskableGraphic
{
    [SerializeField] private Color keyholeColor = new Color32(235, 225, 196, 255);

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect bounds = GetPixelAdjustedRect();

        const int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float start = i * Mathf.PI / segments;
            float end = (i + 1) * Mathf.PI / segments;
            int vertex = mesh.currentVertCount;
            AddVertex(mesh, bounds, new Vector2(0.5f + Mathf.Cos(start) * 0.29f, 0.64f + Mathf.Sin(start) * 0.29f), color);
            AddVertex(mesh, bounds, new Vector2(0.5f + Mathf.Cos(end) * 0.29f, 0.64f + Mathf.Sin(end) * 0.29f), color);
            AddVertex(mesh, bounds, new Vector2(0.5f + Mathf.Cos(end) * 0.18f, 0.64f + Mathf.Sin(end) * 0.18f), color);
            AddVertex(mesh, bounds, new Vector2(0.5f + Mathf.Cos(start) * 0.18f, 0.64f + Mathf.Sin(start) * 0.18f), color);
            mesh.AddTriangle(vertex, vertex + 1, vertex + 2);
            mesh.AddTriangle(vertex, vertex + 2, vertex + 3);
        }
        AddQuad(mesh, bounds, 0.21f, 0.32f, 0.47f, 0.64f, color);
        AddQuad(mesh, bounds, 0.68f, 0.79f, 0.47f, 0.64f, color);
        AddQuad(mesh, bounds, 0.12f, 0.88f, 0.08f, 0.57f, color);

        Color hole = keyholeColor;
        hole.a *= color.a;
        int center = mesh.currentVertCount;
        AddVertex(mesh, bounds, new Vector2(0.5f, 0.36f), hole);
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            AddVertex(mesh, bounds, new Vector2(0.5f + Mathf.Cos(angle) * 0.075f, 0.36f + Mathf.Sin(angle) * 0.075f), hole);
            if (i > 0)
                mesh.AddTriangle(center, center + i, center + i + 1);
        }
        AddQuad(mesh, bounds, 0.465f, 0.535f, 0.19f, 0.35f, hole);
    }

    private static void AddQuad(VertexHelper mesh, Rect bounds, float left, float right, float bottom, float top, Color tint)
    {
        int vertex = mesh.currentVertCount;
        AddVertex(mesh, bounds, new Vector2(left, bottom), tint);
        AddVertex(mesh, bounds, new Vector2(left, top), tint);
        AddVertex(mesh, bounds, new Vector2(right, top), tint);
        AddVertex(mesh, bounds, new Vector2(right, bottom), tint);
        mesh.AddTriangle(vertex, vertex + 1, vertex + 2);
        mesh.AddTriangle(vertex, vertex + 2, vertex + 3);
    }

    private static void AddVertex(VertexHelper mesh, Rect bounds, Vector2 point, Color tint)
    {
        mesh.AddVert(new Vector3(bounds.xMin + point.x * bounds.width, bounds.yMin + point.y * bounds.height),
            tint, Vector2.zero);
    }
}
