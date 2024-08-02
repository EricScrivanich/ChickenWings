using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneManagerSO : ScriptableObject
{
    [SerializeField] private List<string> Levels_Scene;
    [SerializeField] private List<Image> Levels_UI;
    [SerializeField] private List<string> Gamemodes_Scene;
    [SerializeField] private List<Scene> Gamemodes_UI;


    public void LoadLevel(int levelNumber)
    {
        if (levelNumber == 0 || levelNumber >= Levels_Scene.Count) return;
        SceneManager.LoadScene(Levels_Scene[levelNumber]);

    }
}
