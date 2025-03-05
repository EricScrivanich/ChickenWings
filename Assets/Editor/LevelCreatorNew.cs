using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelCreatorNew : Editor
{
    private LevelData Parent;
    public GameObject recorder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnInspectorGUI()
    {
        Parent = (LevelData)target;
        recorder = GameObject.Find("SetupRecorderParent");

        base.OnInspectorGUI(); // Draw 
        if (GUILayout.Button("Test Record", GUILayout.Height(40)))
        {
            var w = new WaveData();
            var objs = recorder.GetComponentsInChildren<IRecordableObject>();


            w.Data = new RecordedDataStruct[objs.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                // Parent.CreateNewWave()
            }

        }
    }

}
