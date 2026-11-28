using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjTrigerNear))]
public sealed class ObjTrigerNearEditor : Editor
{
    private static readonly string[] Modes = { "Radius", "Collider" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

        SerializedProperty colliderMode = serializedObject.FindProperty("colliderCheck");
        EditorGUI.BeginChangeCheck();
        int mode = EditorGUILayout.Popup("Detection Mode", colliderMode.boolValue ? 1 : 0, Modes);
        if (EditorGUI.EndChangeCheck())
            colliderMode.boolValue = mode == 1;

        if (!colliderMode.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceRadius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionOrigin"));
        }
        else
        {
            EditorGUILayout.HelpBox("Use an Is Trigger collider on this GameObject, or a child collider with its Rigidbody on this object. At least one side needs a Rigidbody.", MessageType.Info);
        }

        SerializedProperty targets = serializedObject.FindProperty("enterColliders");
        EditorGUILayout.PropertyField(targets, true);
        if (targets.arraySize == 0)
            EditorGUILayout.HelpBox("Add the collider(s) you want to detect to Enter Colliders, such as the player's collider.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjEnteredEvt"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjExitedEvt"));
        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(serializedObject.FindProperty("objEntered"), new GUIContent("Object Detected"));

        serializedObject.ApplyModifiedProperties();
    }
}
