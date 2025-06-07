using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CageAttatchment))]
public class CageAttatchmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CageAttatchment obj = (CageAttatchment)target;

        if (GUILayout.Button(obj.showGizmo ? "Hide Offset Gizmo" : "Show Offset Gizmo"))
        {
            Undo.RecordObject(obj, "Toggle Gizmo Visibility");
            obj.showGizmo = !obj.showGizmo;
            EditorUtility.SetDirty(obj);
        }
    }

    void OnSceneGUI()
    {
        CageAttatchment obj = (CageAttatchment)target;

        if (!obj.showGizmo) return;

        Vector3 worldPos = obj.transform.position + (Vector3)obj.offset;

        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(obj, "Move Offset Handle");
            obj.offset = newWorldPos - obj.transform.position;
        }

        Handles.color = Color.green;
        Handles.DrawDottedLine(obj.transform.position, worldPos, 3f);
        Handles.Label(worldPos, $"Offset: {obj.offset}");
    }
}