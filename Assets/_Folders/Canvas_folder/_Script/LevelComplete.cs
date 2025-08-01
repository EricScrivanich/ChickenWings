using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private TextMeshProUGUI levelTitle;
    [SerializeField] private ChallengesUIManager challengeUIManager;
    // Start is called before the first frame update
    private void OnEnable()
    {
        levelTitle.text = lvlID.LevelTitle;

        if (challengeUIManager != null)
        {
            var levelData = LevelDataConverter.instance.ReturnLevelData();
            challengeUIManager.ShowChallengesForLevelPicker(levelData.GetLevelChallenges(false, null),LevelDataConverter.instance.ReturnLevelSavedData(), true);
        }

    }
}
