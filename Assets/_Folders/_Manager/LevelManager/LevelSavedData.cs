

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSavedData
{

    public string LevelName;
    public Vector2Int LevelNumber;
    public int LevelID;
    public ushort FurthestCompletionEasy;
    public bool CompletedLevelEasy;

    public ushort FurthestCompletion;
    public bool CompletedLevel;

    public bool MasteredLevel;

    public bool[] ChallengeCompletion;

    public LevelSavedData(LevelData data)
    {
        LevelName = data.LevelName;
        LevelNumber = new Vector2Int(data.levelWorldAndNumber.y, data.levelWorldAndNumber.z);
        CompletedLevel = false;
        MasteredLevel = false;
        ChallengeCompletion = new bool[data.GetLevelChallenges(false, null).NumberOfChallenges]; // Initialized to false by default
    }

}

[System.Serializable]
public class SavedLevelDataByWorld
{
    public short LevelWorld;
    public LevelSavedData[] levels;

    public List<short> unlockedMapItems = new List<short>();

    public short NextUnlockedItem = -1;

    public void AddUnlockableMapItems(short index)
    {
        if (index == -1)
        {
            NextUnlockedItem = -1;
            return;
        }
        if (unlockedMapItems == null)
        {
            unlockedMapItems = new List<short>();
        }
        if (!unlockedMapItems.Contains(index))
        {
            unlockedMapItems.Add(index);
            NextUnlockedItem = index;
        }

    }

    public SavedLevelDataByWorld(short levelWorld)
    {
        LevelWorld = levelWorld;

    }
    public void AddLevel(LevelSavedData level)
    {
        Debug.Log("Adding level: " + level.LevelName + " to world: " + LevelWorld);
        if (levels == null)
        {
            levels = new LevelSavedData[] { level };
        }
        else
        {
            List<LevelSavedData> levelList = new List<LevelSavedData>(levels);
            levelList.Add(level);
            // Sort levels by by level number x then y
            levelList.Sort((a, b) =>
            {
                if (a.LevelNumber.x != b.LevelNumber.x)
                {
                    return a.LevelNumber.x.CompareTo(b.LevelNumber.x);
                }
                return a.LevelNumber.y.CompareTo(b.LevelNumber.y);
            });
            levels = levelList.ToArray();
        }
    }

    public int ReturnAllCompletedChallenges()
    {
        int completedChallenges = 0;
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No levels found in this world: " + LevelWorld);
            return completedChallenges;
        }
        for (int i = 0; i < levels.Length; i++)
        {
            for (int j = 0; j < levels[i].ChallengeCompletion.Length; j++)
            {
                if (levels[i].ChallengeCompletion[j])
                {
                    completedChallenges++;
                }
            }
        }
        return completedChallenges;
    }
    public int ReturnAllMasteredLevels()
    {
        int masteredLevels = 0;
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No levels found in this world: " + LevelWorld);
            return masteredLevels;
        }
        for (int i = 0; i < levels.Length; i++)
        {
            for (int j = 0; j < levels[i].ChallengeCompletion.Length; j++)
            {
                if (levels[i].MasteredLevel)
                {
                    masteredLevels++;
                }
            }
        }
        return masteredLevels;

    }
}


[System.Serializable]
public class GameData
{

    public List<LevelSavedData> Levels = new List<LevelSavedData>();
}

[System.Serializable]
public class TemporaryAllLevelCheckPointData
{
    public Vector3Int LevelAndWorldNumber;
    public int LevelDifficulty;
    public TemporaryLevelCheckPointData[] LevelCheckPointData = null;

    public bool CheckIfSameLevel(Vector3Int levelAndWorldNumber, int levelDifficulty)
    {
        Debug.LogError("Checking if same level: " + LevelAndWorldNumber + " " + LevelDifficulty + " == " + levelAndWorldNumber + " " + levelDifficulty);
        return LevelAndWorldNumber == levelAndWorldNumber && LevelDifficulty == levelDifficulty;
    }

    public bool CheckIfCurrentCheckPoint(short check)
    {
        if (check == -1)
        {
            if (LevelCheckPointData == null || LevelCheckPointData.Length == 0)
            {
                return false;
            }
            else return true;
        }
        return ((ushort)check == LevelCheckPointData[LevelCheckPointData.Length - 1].CurrentCheckPoint);
    }

    public TemporaryAllLevelCheckPointData RemoveCheckPoints(short check)
    {
        if (check == -1)
        {
            LevelCheckPointData = null;
            return this;
        }
        List<TemporaryLevelCheckPointData> newCheckPoints = new List<TemporaryLevelCheckPointData>();

        foreach (var checkPoint in LevelCheckPointData)
        {
            if (checkPoint.CurrentCheckPoint <= check)
            {
                newCheckPoints.Add(checkPoint);
            }
        }
        // order list by current checkpoint
        newCheckPoints.Sort((a, b) => a.CurrentCheckPoint.CompareTo(b.CurrentCheckPoint));

        LevelCheckPointData = newCheckPoints.ToArray();
        return this;
    }
    public TemporaryAllLevelCheckPointData AddCheckPoint(LevelChallenges data)
    {
        List<TemporaryLevelCheckPointData> newCheckPoints = new List<TemporaryLevelCheckPointData>();
        if (LevelCheckPointData != null && LevelCheckPointData.Length > 0)
            foreach (var checkPoint in LevelCheckPointData)
            {
                newCheckPoints.Add(checkPoint);

            }

        newCheckPoints.Add(new TemporaryLevelCheckPointData(data));

        // order list by current checkpoint
        newCheckPoints.Sort((a, b) => a.CurrentCheckPoint.CompareTo(b.CurrentCheckPoint));

        LevelCheckPointData = newCheckPoints.ToArray();
        return this;
    }

    public TemporaryLevelCheckPointData ReturnCheckPointData(ushort check, bool returnLast = false)
    {
        if (returnLast)
        {
            if (LevelCheckPointData == null || LevelCheckPointData.Length == 0)
            {
                Debug.LogError("No Checkpoints found for this level: " + LevelAndWorldNumber + " " + LevelDifficulty);

                return null;
            }
            return LevelCheckPointData[LevelCheckPointData.Length - 1];
        }
        else
        {
            foreach (var checkPoint in LevelCheckPointData)
            {
                if (checkPoint.CurrentCheckPoint == check)
                {
                    return checkPoint;
                }
            }
            return null;
        }


    }
}






[System.Serializable]
public class TemporaryLevelCheckPointData
{



    // public int CagesPresent;
    public ushort CurrentCheckPoint;
    public float totalAddedRandomTime;
 
    public short CurrentLives;
    public short[] CurrentAmmos;

    public Vector3Int[] KilledPigs;
    public int[] PlayerInputs;
    public int[] CompletedRings;
    public int[] CurrentEggedBarns;
    public byte cageType;
    public float totalAddedTime;

    public TemporaryLevelCheckPointData(LevelChallenges data)
    {


        CurrentCheckPoint = data.CurrentCheckPoint;
       
        CurrentLives = (short)data.Lives;
        cageType = data.cageType;


        if (data.Ammos != null)
            CurrentAmmos = (short[])data.Ammos.Clone();




        if (data.KilledPigs != null)
            KilledPigs = data.KilledPigs.ToArray();
        if (data.PlayerInputs != null)
            PlayerInputs = data.PlayerInputs.ToArray();
        if (data.CompletedRings != null)
            CompletedRings = data.CompletedRings.ToArray();
        if (data.BarnsHitWithEgg != null)
            CurrentEggedBarns = data.BarnsHitWithEgg.ToArray();

    }



}
