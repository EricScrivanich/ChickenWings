using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ILevelPickerPathObject), true)]
[CanEditMultipleObjects]
public class LevelPickerZoom : Editor
{
    // Per-object UI state (editor-only, not serialized)
    private static readonly Dictionary<int, bool> s_ShowGizmo = new();
    private static readonly Dictionary<int, bool> s_ShowMoveHandle = new();
    private static readonly Dictionary<int, bool> s_ShowOutline = new();

    // Static copy buffer
    private static bool s_HasCopy = false;
    private static Vector4 s_CopyBuffer;

    public override void OnInspectorGUI()
    {
        if (lpMan == null) lpMan = GameObject.Find("LevelPickManager").GetComponent<LevelPickerManager>();
        // Draw the component’s normal inspector
        DrawDefaultInspector();

        int id = target.GetInstanceID();
        if (!s_ShowGizmo.ContainsKey(id)) s_ShowGizmo[id] = true;
        if (!s_ShowMoveHandle.ContainsKey(id)) s_ShowMoveHandle[id] = true;
        if (!s_ShowOutline.ContainsKey(id)) s_ShowOutline[id] = true;

        // --- Camera controls (sliders) ---
        var picker = (ILevelPickerPathObject)target;
        Vector4 cam = picker.CameraPositionAndOrhtoSize;

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Camera View Controls", EditorStyles.boldLabel);
        // simple 


        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Reset Data", GUILayout.Height(15)))
            {
                picker.CameraPositionAndOrhtoSize = lpMan.BaseCameraData;
                lpMan.SetHillPos(picker.CameraPositionAndOrhtoSize);
                Camera.main.transform.position = new Vector3(picker.CameraPositionAndOrhtoSize.x, picker.CameraPositionAndOrhtoSize.y, picker.CameraPositionAndOrhtoSize.z);
                Camera.main.orthographicSize = picker.CameraPositionAndOrhtoSize.w;
                EditorUtility.SetDirty(picker);
                SceneView.RepaintAll();
                // Whatever you want to reset
            }
            if (GUILayout.Button("Reset Cam", GUILayout.Height(15)))
            {

                lpMan.SetHillPos(Vector4.zero);
                Camera.main.transform.position = new Vector3(0, 0, -10);
                Camera.main.orthographicSize = lpMan.BaseCameraData.w;
                EditorUtility.SetDirty(picker);
                SceneView.RepaintAll();
                // Whatever you want to reset
            }
            if (GUILayout.Button("Set Additional Parralax Positions", GUILayout.Height(15)))
            {

                lpMan.SetAdditionalObjectPostions();
                EditorUtility.SetDirty(lpMan);
                SceneView.RepaintAll();
                // Whatever you want to reset
            }
        }

        EditorGUI.BeginChangeCheck();
        float newX = EditorGUILayout.Slider(new GUIContent("X", "CameraPositionAndOrhtoSize.x"),
                                           cam.x, -15, 15);
        float newY = EditorGUILayout.Slider(new GUIContent("Y", "CameraPositionAndOrhtoSize.y"),
   cam.y, -5, 5);
        // Z slider
        float newZ = EditorGUILayout.Slider(new GUIContent("Z", "CameraPositionAndOrhtoSize.z"),
                                            cam.z, -10f, -4f);
        // Ortho size slider (w)
        float newOrtho = EditorGUILayout.Slider(new GUIContent("Ortho Size", "CameraPositionAndOrhtoSize.w"),
                                               cam.w, 0.5f, 8f);



        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(picker, "Adjust Camera View Sliders");
            picker.CameraPositionAndOrhtoSize = new Vector4(newX, newY, newZ, newOrtho);
            lpMan.SetHillPos(picker.CameraPositionAndOrhtoSize);
            Camera.main.transform.position = new Vector3(picker.CameraPositionAndOrhtoSize.x, picker.CameraPositionAndOrhtoSize.y, picker.CameraPositionAndOrhtoSize.z);
            Camera.main.orthographicSize = picker.CameraPositionAndOrhtoSize.w;
            EditorUtility.SetDirty(picker);
            SceneView.RepaintAll();
        }

        EnsureLpMan();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Hills (LevelPickerManager)", EditorStyles.boldLabel);

        // Allow overriding the auto-found manager
        EditorGUI.BeginChangeCheck();
        lpMan = (LevelPickerManager)EditorGUILayout.ObjectField(
            new GUIContent("Manager", "LevelPickerManager in the scene"),
            lpMan, typeof(LevelPickerManager), true);
        if (EditorGUI.EndChangeCheck())
        {
            // nothing else; just let the reference change
        }

        if (lpMan == null)
        {
            EditorGUILayout.HelpBox(
                "No LevelPickerManager found. Place one in the scene (named 'LevelPickManager') " +
                "or drag it into the Manager field above.", MessageType.Warning);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            float xHill = EditorGUILayout.Slider(new GUIContent("xHillMult"), lpMan.xHillMult, -10f, 10f);
            float yHill = EditorGUILayout.Slider(new GUIContent("yHillMult"), lpMan.yHillMult, -10f, 10f);
            float sHill = EditorGUILayout.Slider(new GUIContent("scaleHillMult"), lpMan.scaleHillMult, 0f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(lpMan, "Edit Hill Multipliers");
                lpMan.xHillMult = xHill;
                lpMan.yHillMult = yHill;
                lpMan.scaleHillMult = sHill;
                Camera.main.transform.position = new Vector3(picker.CameraPositionAndOrhtoSize.x, picker.CameraPositionAndOrhtoSize.y, picker.CameraPositionAndOrhtoSize.z);
                Camera.main.orthographicSize = picker.CameraPositionAndOrhtoSize.w;
                lpMan.SetHillPos(picker.CameraPositionAndOrhtoSize);
                EditorUtility.SetDirty(lpMan);
                SceneView.RepaintAll();
            }
        }

        // --- Gizmo toggles ---
        EditorGUILayout.Space(6);
        using (new EditorGUILayout.HorizontalScope())
        {
            s_ShowGizmo[id] = GUILayout.Toggle(
                s_ShowGizmo[id], s_ShowGizmo[id] ? "Hide Gizmo" : "Show Gizmo",
                "Button", GUILayout.Height(22));

            s_ShowOutline[id] = GUILayout.Toggle(
                s_ShowOutline[id], s_ShowOutline[id] ? "Hide Outline" : "Show Outline",
                "Button", GUILayout.Height(22));

            s_ShowMoveHandle[id] = GUILayout.Toggle(
                s_ShowMoveHandle[id], s_ShowMoveHandle[id] ? "Disable Move" : "Enable Move",
                "Button", GUILayout.Height(22));
        }

        // --- Copy / Paste ---
        EditorGUILayout.Space(6);
        using (new EditorGUILayout.HorizontalScope())
        {
            bool singleSelection = targets != null && targets.Length == 1;
            using (new EditorGUI.DisabledScope(!singleSelection))
            {
                if (GUILayout.Button(new GUIContent("Copy", "Copy CameraPositionAndOrhtoSize from this object"),
                                     GUILayout.Height(22)))
                {
                    var src = (ILevelPickerPathObject)target;
                    s_CopyBuffer = src.CameraPositionAndOrhtoSize;
                    s_HasCopy = true;
                    EditorGUIUtility.systemCopyBuffer =
                        $"{s_CopyBuffer.x},{s_CopyBuffer.y},{s_CopyBuffer.z},{s_CopyBuffer.w}";
                    ShowTempNotification("Copied camera vector");
                }
            }

            using (new EditorGUI.DisabledScope(!s_HasCopy))
            {
                if (GUILayout.Button(new GUIContent("Paste", "Paste copied CameraPositionAndOrhtoSize to selection"),
                                     GUILayout.Height(22)))
                {
                    foreach (var t in targets)
                    {
                        if (t is ILevelPickerPathObject dst)
                        {
                            Undo.RecordObject(dst, "Paste Camera Vector");
                            dst.CameraPositionAndOrhtoSize = s_CopyBuffer;
                            EditorUtility.SetDirty(dst);
                        }
                    }
                    SceneView.RepaintAll();
                    ShowTempNotification(targets.Length > 1 ? "Pasted to selection" : "Pasted");
                }
            }
        }



        EditorGUILayout.HelpBox(
            "Gizmo draws a box for CameraPositionAndOrhtoSize (x,y,z,w) using Game View aspect.\n" +
            "Toggle the gizmo/outline/move above. Copy works with a single selection; Paste applies to all selected.",
            MessageType.Info);


        if (GUI.changed)
            SceneView.RepaintAll();
    }
    private void EnsureLpMan()
    {
        if (lpMan != null) return;

        // Try by name first (your original pattern)
        var go = GameObject.Find("LevelPickManager");
        if (go != null) lpMan = go.GetComponent<LevelPickerManager>();

        // Fallback: find any in the scene
        if (lpMan == null) lpMan = Object.FindObjectOfType<LevelPickerManager>();
    }

    private static void ShowTempNotification(string msg)
    {
        // Small nicety: show a scene view notification (best-effort)
        var sv = SceneView.lastActiveSceneView;
        if (sv != null)
        {
            sv.ShowNotification(new GUIContent(msg));
        }
    }
    private LevelPickerManager lpMan;
    private void OnSceneGUI()
    {

        var picker = (ILevelPickerPathObject)target;
        int id = picker.GetInstanceID();

        if (!s_ShowGizmo.TryGetValue(id, out bool show) || !show)
            return;

        // Camera-ish data
        Vector3 camPos = new Vector3(
            picker.CameraPositionAndOrhtoSize.x,
            picker.CameraPositionAndOrhtoSize.y,
            picker.CameraPositionAndOrhtoSize.z
        );
        float orthoSize = picker.CameraPositionAndOrhtoSize.w;

        // Aspect
        float aspect = GetGameViewAspectFallback();

        float camHeight = Mathf.Max(0.0001f, orthoSize * 2f);
        float camWidth = camHeight * aspect;

        // Rect verts in XY plane
        Vector3 bl = camPos + new Vector3(-camWidth * 0.5f, -camHeight * 0.5f, 0f);
        Vector3 tl = camPos + new Vector3(-camWidth * 0.5f, camHeight * 0.5f, 0f);
        Vector3 tr = camPos + new Vector3(camWidth * 0.5f, camHeight * 0.5f, 0f);
        Vector3 br = camPos + new Vector3(camWidth * 0.5f, -camHeight * 0.5f, 0f);
        var verts = new[] { bl, tl, tr, br };

        // Fill + optional outline
        Handles.color = new Color(0f, 1f, 0f, 0.08f);
        Handles.DrawAAConvexPolygon(verts);

        if (s_ShowOutline.TryGetValue(id, out bool outline) && outline)
        {
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(3f, new[] { bl, tl, tr, br, bl });
        }

        // Label
        GUIStyle label = new GUIStyle(EditorStyles.boldLabel);
        label.normal.textColor = Color.green;
        Handles.Label(camPos + Vector3.up * (camHeight * 0.55f),
            $"Cam Rect  W:{camWidth:0.###}  H:{camHeight:0.###}  Ortho:{orthoSize:0.###}  AR:{aspect:0.###}",
            label);

        // Optional move handle for the rect center
        if (s_ShowMoveHandle.TryGetValue(id, out bool canMove) && canMove)
        {
            EditorGUI.BeginChangeCheck();
            Handles.color = Color.green;
            Vector3 newCenter = Handles.PositionHandle(camPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(picker, "Move Camera View Center");
                var v = picker.CameraPositionAndOrhtoSize;
                picker.CameraPositionAndOrhtoSize = new Vector4(newCenter.x, newCenter.y, newCenter.z, v.w);
                Camera.main.transform.position = new Vector3(newCenter.x, newCenter.y, newCenter.z);
                Camera.main.orthographicSize = v.w;
                lpMan.SetHillPos(picker.CameraPositionAndOrhtoSize);
                EditorUtility.SetDirty(Camera.main);
                EditorUtility.SetDirty(picker);
            }
        }
    }

    private static float GetGameViewAspectFallback()
    {
        // Try Game View (reflection). If it fails, use Scene View camera aspect or default 16:9.
        Vector2 gv = HandlesExtensions.GetMainGameViewSize();
        if (gv.x > 1f && gv.y > 1f)
            return gv.x / gv.y;

        var sv = SceneView.lastActiveSceneView;
        if (sv != null && sv.camera != null && sv.camera.pixelHeight > 0)
            return (float)sv.camera.pixelWidth / sv.camera.pixelHeight;

        return 16f / 9f;
    }
}

/// <summary>
/// Reflection helper to read the current GameView target size
/// </summary>
/// /// 
public static class HandlesExtensions
{
    private static System.Reflection.MethodInfo getSizeOfMainGameView;

    public static Vector2 GetMainGameViewSize()
    {
        if (getSizeOfMainGameView == null)
        {
            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            getSizeOfMainGameView = gameViewType?.GetMethod(
                "GetMainGameViewTargetSize",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        if (getSizeOfMainGameView != null)
        {
            object res = getSizeOfMainGameView.Invoke(null, null);
            if (res is Vector2 v) return v;
        }
        return Vector2.zero;
    }


}
