#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

public class PickLevelAddNewLevelEditorOnly : MonoBehaviour
{

    public Vector3Int levelNumbers { get; private set; }
    public string levelName { get; private set; }
    [SerializeField] private Button addButton;
    private bool dataIsValid = false;

    [SerializeField] private PickLevelEditorOnly parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        dataIsValid = false;
        levelNumbers = Vector3Int.zero;
        addButton.interactable = false;
        levelName = "None";

    }

    // Update is called once per frame
    public void SetWorldNumber(string w)
    {
        int worldNumber = int.Parse(w);

        levelNumbers = new Vector3Int(worldNumber, levelNumbers.y, levelNumbers.z);
        CheckAddButton();

    }
    public void SetLevelNumber(string l)
    {
        int levelNumber = int.Parse(l);
        levelNumbers = new Vector3Int(levelNumbers.x, levelNumber, levelNumbers.z);
        CheckAddButton();

    }
    public void SetSpecialNumber(string s)
    {
        int specialNumber = int.Parse(s);

        levelNumbers = new Vector3Int(levelNumbers.x, levelNumbers.y, specialNumber);
        CheckAddButton();

    }
    public void SetLevelName(string name)
    {
        levelName = name;
        CheckAddButton();

    }

    private void CheckAddButton()
    {
        if (PickLevelEditorOnly.ReturnLevelName(levelName,levelNumbers,false) != null )
           
        {
            dataIsValid = true;
            addButton.interactable = true;
        }
        else
        {
            dataIsValid = false;
            addButton.interactable = false;
        }


    }

    public void ClickAddButton()
    {
        if (dataIsValid)
            parent.AddNewLevel(levelNumbers, levelName);

    }



}
#endif