
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

public class PickLevelLevelButton : MonoBehaviour
{

    private string levelName;
    private PickLevelEditorOnly parent;
    private Image image;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.grey;

    }
    public void SetLevelName(PickLevelEditorOnly p, string buttonName, string name)
    {
        parent = p;
        levelName = name;

        GetComponentInChildren<TMPro.TMP_Text>().text = buttonName;
    }

    // Update is called once per frame
    public void Press()
    {
        if (parent != null)
        {
            parent.ChooseLevel(levelName);
        }
        else
        {
            Debug.LogWarning("Parent is not set for PickLevelLevelButton");
        }

    }

    public void CheckIfSelected(string name)
    {
        if (name == levelName)
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.grey;
        }
    }
}
#endif