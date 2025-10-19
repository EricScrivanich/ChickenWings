using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationRenamerWindow : EditorWindow
{
    private AnimationClip clip;
    private string searchTerm = "";
    private string insertBeforeKeyword = "";
    private bool showPreview = true;
    private bool keywordMode = true;

    [MenuItem("Window/Animation Renamer")]
    public static void ShowWindow()
    {
        GetWindow<AnimationRenamerWindow>("Animation Renamer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Animation Clip Renamer", EditorStyles.boldLabel);
        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", clip, typeof(AnimationClip), false);

        if (clip == null) return;

        EditorGUILayout.Space();
        keywordMode = EditorGUILayout.Toggle("Keyword Insert Mode", keywordMode);

        if (keywordMode)
        {
            searchTerm = EditorGUILayout.TextField("Keyword to Find", searchTerm);
            insertBeforeKeyword = EditorGUILayout.TextField("Insert Before (New Folder)", insertBeforeKeyword);
        }
        else
        {
            searchTerm = EditorGUILayout.TextField("Search", searchTerm);
            insertBeforeKeyword = EditorGUILayout.TextField("Replace", insertBeforeKeyword);
        }

        EditorGUILayout.Space();
        showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);

        EditorGUILayout.Space();
        if (GUILayout.Button("Preview Rename"))
        {
            showPreview = true;
        }

        if (GUILayout.Button("Apply Rename"))
        {
            RenameBindings();
        }

        if (showPreview)
        {
            DrawPreview();
        }
    }

    private void DrawPreview()
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);
        EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);

        foreach (var b in bindings)
        {
            string newPath = GetNewPath(b.path);
            if (newPath != b.path)
                EditorGUILayout.LabelField($"{b.path}  →  {newPath}");
        }
    }

    private void RenameBindings()
    {
        if (clip == null) return;

        Undo.RecordObject(clip, "Rename Animation Paths");
        var bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var b in bindings)
        {
            string newPath = GetNewPath(b.path);
            if (newPath == b.path) continue;

            var curve = AnimationUtility.GetEditorCurve(clip, b);
            var newBinding = b;
            newBinding.path = newPath;

            AnimationUtility.SetEditorCurve(clip, b, null);
            AnimationUtility.SetEditorCurve(clip, newBinding, curve);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Animation paths updated!");
    }

    private string GetNewPath(string oldPath)
    {
        if (!keywordMode)
            return oldPath.Replace(searchTerm, insertBeforeKeyword);

        if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(insertBeforeKeyword))
            return oldPath;

        string[] parts = oldPath.Split('/');
        List<string> newParts = new List<string>();

        foreach (var p in parts)
        {
            if (p.Contains(searchTerm))
                newParts.Add(insertBeforeKeyword);
            newParts.Add(p);
        }

        return string.Join("/", newParts);
    }
}