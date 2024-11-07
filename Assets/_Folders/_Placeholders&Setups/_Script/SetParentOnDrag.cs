using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]

[SelectionBase]
public class SetParentOnDrag : MonoBehaviour
{
    private Transform parentTransform; // The parent to which this object should be attached

    [SerializeField] private bool isCollectable;

    void OnValidate()
    {
        // Check if the game object is part of the scene and not a prefab asset
        if (!EditorApplication.isPlaying && !EditorUtility.IsPersistent(this))
        {
            if (isCollectable) parentTransform = GameObject.Find("SetupRecorderCollectable").transform;
            else parentTransform = GameObject.Find("SetupRecorderEnemy").transform;
            if (parentTransform != null && transform.parent != parentTransform)
            {
                transform.SetParent(parentTransform);
                Debug.Log($"Set {gameObject.name}'s parent to {parentTransform.name}");
            }
        }
    }

    void OnEnable()
    {
        // Check if the game object is part of the scene and not a prefab asset
        if (!EditorApplication.isPlaying && !EditorUtility.IsPersistent(this))
        {
            if (parentTransform != null && transform.parent != parentTransform)
            {
                transform.SetParent(parentTransform);
                Debug.Log($"Set {gameObject.name}'s parent to {parentTransform.name}");
            }
        }
    }
}
#endif