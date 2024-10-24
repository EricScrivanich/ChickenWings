using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneManagerSO : ScriptableObject
{
    [SerializeField] private List<string> Levels_Scene;
    [SerializeField] private List<string> Levels_Name;
    [SerializeField] private List<string> OtherGameModes_Scene;
    [SerializeField] private List<string> OtherGameModes_Name;
    [SerializeField] private List<Image> Levels_UI;
    [SerializeField] private List<string> Gamemodes_Scene;
    [SerializeField] private List<Scene> Gamemodes_UI;


    public void LoadLevel(int levelNumber)
    {
        if (levelNumber == 0 || levelNumber >= Levels_Scene.Count) return;

        if (Time.timeScale == 0) Time.timeScale = 1;

        if (levelNumber == -1)
            SceneManager.LoadScene("BasicsPig");
        else
            SceneManager.LoadScene(Levels_Scene[levelNumber]);

    }

    public void LoadGamemode(int type)
    {
        if (type >= OtherGameModes_Scene.Count) return;
        if (Time.timeScale == 0) Time.timeScale = 1;

        SceneManager.LoadScene(OtherGameModes_Scene[type]);




    }

    public string ReturnLevelName(int index)
    {
        return Levels_Name[index];
    }
    public int LevelsCount()
    {
        return Levels_Scene.Count;
    }

    public string ReturnGameModeName(int index)
    {
        return OtherGameModes_Name[index];
    }
    public int GameModesCount()
    {
        return OtherGameModes_Scene.Count;
    }
    public string ReturnSceneNameGameMode(int index)
    {
        if (OtherGameModes_Scene[index] != null)
            return OtherGameModes_Scene[index];
        else
            return "none";
    }

    public string ReturnSceneNameLevel(int index)
    {
        if (Levels_Scene[index] != null)
            return Levels_Scene[index];
        else
            return "none";
    }

}
