using UnityEngine;
using UnityEditor;

public class SelectParent : EditorWindow {

    [MenuItem("Edit/Select parent #z")] // % represents Ctrl on Windows/Linux and Cmd on macOS
    static void SelectParentOfObject() {
        if (Selection.activeGameObject != null && Selection.activeGameObject.transform.parent != null) {
            Selection.activeGameObject = Selection.activeGameObject.transform.parent.gameObject;
        }
    }
}