

using System.Collections.Generic;

[System.Serializable]
public class LevelSavedData
{

    public int LevelID;
    public bool CompletedLevel;

    public bool MasteredLevel;

    public bool[] ChallengeCompletion;

    public LevelSavedData(int levelId, int challengeCount)
    {
        LevelID = levelId;
        CompletedLevel = false;
        MasteredLevel = false;
        ChallengeCompletion = new bool[challengeCount]; // Initialized to false by default
    }

}


[System.Serializable]
public class GameData
{
    public List<LevelSavedData> Levels = new List<LevelSavedData>();
}
