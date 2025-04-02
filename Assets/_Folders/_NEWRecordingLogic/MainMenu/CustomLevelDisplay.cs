using UnityEngine;
using TMPro;

public class CustomLevelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private LevelScrollMenu levelScrollMenu;
    private string levelName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(LevelScrollMenu parent, string name)
    {
        levelScrollMenu = parent;
        levelName = name;
        text.text = name;
    }

    public void OnClick()
    {
        levelScrollMenu.SetSelected(levelName);
    }
}
