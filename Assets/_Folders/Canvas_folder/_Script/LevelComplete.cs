using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private TextMeshProUGUI levelTitle;
    // Start is called before the first frame update
    private void OnEnable()
    {
        levelTitle.text = lvlID.LevelTitle;

    }
}
