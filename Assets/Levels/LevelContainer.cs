using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "ScriptableObjects/LevelContainer")]
public class LevelContainer : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public LevelData[] levels { get; private set; }



#if UNITY_EDITOR
    public void AddLevel(LevelData level)
    {
        if (levels == null)
        {
            levels = new LevelData[] { level };
        }
        else
        {

            List<LevelData> newLevels = new List<LevelData>();
            Debug.Log("Levels length: " + levels.Length);

            for (int i = 0; i < levels.Length; i++)
            {
                newLevels.Add(levels[i]);
            }

            if (level != null)
            {
                newLevels.Add(level);
                Debug.Log("Added level 2: " + level.LevelName);
            }

            // sort new levels by levelWorldAndNumber.x, levelWorldAndNumber.y, levelWorldAndNumber.z then by levelName
            newLevels.Sort((a, b) =>
            {
                int worldComparison = a.levelWorldAndNumber.x.CompareTo(b.levelWorldAndNumber.x);
                if (worldComparison != 0) return worldComparison;

                int levelComparison = a.levelWorldAndNumber.y.CompareTo(b.levelWorldAndNumber.y);
                if (levelComparison != 0) return levelComparison;

                int specialComparison = a.levelWorldAndNumber.z.CompareTo(b.levelWorldAndNumber.z);
                if (specialComparison != 0) return specialComparison;

                return string.Compare(a.LevelName, b.LevelName, System.StringComparison.Ordinal);
            });

            levels = newLevels.ToArray();

        }
    }

    public bool CheckNullItems()
    {
        bool hasNull = false;
        for (int i = levels.Length - 1; i >= 0; i--)
        {
            if (levels[i] == null)
            {

                hasNull = true;

            }
        }

        if (hasNull)
        {
            List<LevelData> newLevels = new List<LevelData>();
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    newLevels.Add(levels[i]);
                }
            }
            levels = newLevels.ToArray();
            return true;
        }
        else
        {
            return false;
        }

    }
#endif
}
