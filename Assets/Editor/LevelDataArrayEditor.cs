using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDataArrays))]

public class LevelDataArrayEditor : Editor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear Random Data"))
        {
            LevelDataArrays lda = (LevelDataArrays)target;
            lda.posListRand = null;
            lda.floatListRand = null;
            lda.typeListRand = null;
            lda.usedRNGIndices = null;
            lda.randomLogicSizes = null;
            lda.randomSpawnDataTypeObjectTypeAndID = null;
            lda.spawnStepsRandom = null;
            lda.randomSpawnRanges = null;
            lda.randomLogicWaveIndices = null;


            EditorUtility.SetDirty(lda);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    // Update is called once per frame

}
