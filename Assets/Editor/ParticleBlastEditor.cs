using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShotgunParticleID))]
public class ParticleBlastEditor : Editor
{
    private const string Prefix = "ParticleBlastEditor_";

    private float OuterYPosAmount
    {
        get => EditorPrefs.GetFloat(Prefix + "OuterYPosAmount", .38f);
        set => EditorPrefs.SetFloat(Prefix + "OuterYPosAmount", value);
    }

    private float InnerYPosAmount
    {
        get => EditorPrefs.GetFloat(Prefix + "InnerYPosAmount", .17f);
        set => EditorPrefs.SetFloat(Prefix + "InnerYPosAmount", value);
    }

    private float OuterRotateAmount
    {
        get => EditorPrefs.GetFloat(Prefix + "OuterRotateAmount", 20f);
        set => EditorPrefs.SetFloat(Prefix + "OuterRotateAmount", value);
    }

    private float InnerRotateAmount
    {
        get => EditorPrefs.GetFloat(Prefix + "InnerRotateAmount", 10f);
        set => EditorPrefs.SetFloat(Prefix + "InnerRotateAmount", value);
    }

    private int OuterBulletCount
    {
        get => EditorPrefs.GetInt(Prefix + "OuterBulletCount", 5);
        set => EditorPrefs.SetInt(Prefix + "OuterBulletCount", value);
    }

    private int InnerBulletCount
    {
        get => EditorPrefs.GetInt(Prefix + "InnerBulletCount", 2);
        set => EditorPrefs.SetInt(Prefix + "InnerBulletCount", value);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Editable values
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Editor-only Configuration", EditorStyles.boldLabel);

        OuterYPosAmount = EditorGUILayout.FloatField("Outer Y Pos Amount", OuterYPosAmount);
        InnerYPosAmount = EditorGUILayout.FloatField("Inner Y Pos Amount", InnerYPosAmount);
        OuterRotateAmount = EditorGUILayout.FloatField("Outer Rotate Amount", OuterRotateAmount);
        InnerRotateAmount = EditorGUILayout.FloatField("Inner Rotate Amount", InnerRotateAmount);

        OuterBulletCount = EditorGUILayout.IntField("Outer Bullet Count", OuterBulletCount);
        InnerBulletCount = EditorGUILayout.IntField("Inner Bullet Count", InnerBulletCount);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate & Apply Bullet Pattern"))
        {
            ApplyPattern();
        }
    }

    private void ApplyPattern()
    {
        int outerCount = Mathf.Max(1, OuterBulletCount);
        int innerCount = Mathf.Max(0, InnerBulletCount);
        Vector2[] bulletRotationsAndYPositions = new Vector2[outerCount + innerCount];

        // OUTER
        float outerPosStep = OuterYPosAmount / (outerCount > 1 ? (outerCount - 1) / 2f : 1f);
        float outerRotStep = OuterRotateAmount / (outerCount > 1 ? (outerCount - 1) / 2f : 1f);
        float outerCenter = (outerCount - 1) / 2f;

        for (int i = 0; i < outerCount; i++)
        {
            float offsetIndex = i - outerCenter;
            bulletRotationsAndYPositions[i].x = offsetIndex * outerPosStep;
            bulletRotationsAndYPositions[i].y = offsetIndex * outerRotStep;
        }

        // INNER
        float innerPosStep = InnerYPosAmount / (innerCount > 1 ? (innerCount - 1) / 2f : 1f);
        float innerRotStep = InnerRotateAmount / (innerCount > 1 ? (innerCount - 1) / 2f : 1f);
        float innerCenter = (innerCount - 1) / 2f;

        for (int i = 0; i < innerCount; i++)
        {
            float offsetIndex = i - innerCenter;
            bulletRotationsAndYPositions[outerCount + i].x = offsetIndex * innerPosStep;
            bulletRotationsAndYPositions[outerCount + i].y = offsetIndex * innerRotStep;
        }

        // Send to SO
        ShotgunParticleID dataTarget = (ShotgunParticleID)target;
        dataTarget.SetData(bulletRotationsAndYPositions);
        EditorUtility.SetDirty(dataTarget);

        Debug.Log("Bullet data successfully applied to ScriptableObject.");
    }
}