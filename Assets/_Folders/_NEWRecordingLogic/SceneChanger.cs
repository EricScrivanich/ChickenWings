using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public void LoadCreatorScene(bool test)
    {
        if (test)
        {
            SceneManager.LoadScene("LevelPlayer");
        }
        else
        {
            SceneManager.LoadScene("LevelCreator");
        }
    }

}
