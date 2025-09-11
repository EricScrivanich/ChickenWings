using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class LevelScrollMenu : MonoBehaviour
{
    [SerializeField] private GameObject levelPrefab; // Prefab for each level button/item.
    private string currentSelectedLevelTitle;

    private CustomLevelDisplay[] levelButtons;
    private RectTransform rectTransform;
    private int baseHeight = 150;
    private UserCreatedLevels userCreatedLevels;
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject sameLevelNameWarning;
    [SerializeField] private TMP_InputField textField;

    [SerializeField] private GameObject selectedLevelDisplay;
    [SerializeField] private TextMeshProUGUI selectedLevelText;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        inputField.SetActive(false);
        Initialize();
    }

    // Loads the levels from disk and populates the scroll view.
    private void Initialize()
    {
        userCreatedLevels = LevelDataConverter.instance.LoadUserCreatedLevels();
        if (userCreatedLevels == null)
        {
            userCreatedLevels = new UserCreatedLevels();
        }

        // Set the height of the scroll container.
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, baseHeight * userCreatedLevels.UserCreatedLevelNames.Count);
        if (levelButtons != null)
        {
            foreach (var button in levelButtons)
            {
                Destroy(button.gameObject);
            }
        }
        // Initialize the buttons array.
        levelButtons = new CustomLevelDisplay[userCreatedLevels.UserCreatedLevelNames.Count];
        Debug.Log("Level count: " + userCreatedLevels.UserCreatedLevelNames.Count);

        // Instantiate each level button and set its data.
        for (int i = 0; i < userCreatedLevels.UserCreatedLevelNames.Count; i++)
        {
            GameObject levelButton = Instantiate(levelPrefab, transform);
            levelButtons[i] = levelButton.GetComponent<CustomLevelDisplay>();
            levelButtons[i].SetData(this, userCreatedLevels.UserCreatedLevelNames[i]);
        }
    }

    // Called when adding a new level.
    public void AddNewLevel()
    {
        if (string.IsNullOrEmpty(currentLevelToCreateName))
        {
            Debug.Log("No name set");
            return;
        }
        else
        {
            // Check for duplicate level 
            // names.
            currentLevelToCreateName = currentLevelToCreateName.Trim();
            currentLevelToCreateName = currentLevelToCreateName.ToLower();


            foreach (var item in userCreatedLevels.UserCreatedLevelNames)
            {
                if (item == currentLevelToCreateName)
                {
                    sameLevelNameWarning.SetActive(true);
                    return;
                }
            }
            // Add the new level name and update the data file.
            userCreatedLevels.UserCreatedLevelNames.Add(currentLevelToCreateName);
            LevelDataConverter.instance.EditUserCreatedLevels(userCreatedLevels, true);
            // Reinitialize the scroll menu to include the new level.
            Initialize();
            inputField.SetActive(false);
            SetSelected(currentLevelToCreateName);
        }
    }

    private string currentLevelToCreateName;
    private int cleanCount = 0;
    private bool cleanedTextField = false;
    public void TempSaveLevelName(string name)
    {
        // Clean the input string.
        string cleaned = CleanLevelName(name);
        currentLevelToCreateName = cleaned;

        // Update the text field if it’s different from the cleaned version.
        if (textField.text != cleaned)
        {
            // Temporarily remove the listener to prevent recursive calls (if using OnValueChanged).
            textField.onValueChanged.RemoveListener(TempSaveLevelName);
            textField.text = cleaned;
            textField.onValueChanged.AddListener(TempSaveLevelName);
        }
    }
    private string CleanLevelName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        bool lastCharWasSpace = true;

        foreach (char c in input)
        {
            // If the character is a letter or digit, append it.
            if (char.IsLetterOrDigit(c))
            {
                Debug.Log("Adding character: " + c);
                sb.Append(c);
                lastCharWasSpace = false;
            }
            // Explicitly check for a space.
            else if (c == ' ')
            {
                Debug.Log("Space detected");
                if (!lastCharWasSpace)
                {
                    Debug.Log("Adding space");
                    sb.Append(c);
                    lastCharWasSpace = true;
                }
                else
                {
                    Debug.Log("Ignoring extra space");
                }
            }
            // Otherwise, if it's any other kind of whitespace, treat it as a space.
            else if (char.IsWhiteSpace(c))
            {
                Debug.Log("Other whitespace detected: " + c);
                if (!lastCharWasSpace)
                {
                    sb.Append(' ');
                    lastCharWasSpace = true;
                }
            }
            // Ignore any other special character.
        }

        // Trim the final result to remove any leading/trailing spaces.
        return sb.ToString();
    }

    public void SetSelected(string name)
    {

        selectedLevelText.text = name;
        currentSelectedLevelTitle = name;
        if (!selectedLevelDisplay.activeInHierarchy)
            selectedLevelDisplay.SetActive(true);



    }

    public void LoadLevel()
    {
        // TransitionDirector.instance.UndoDestroy();
        Destroy(TransitionDirector.instance.gameObject);
        PlayerPrefs.SetString("LevelCreatorPath", currentSelectedLevelTitle);
        PlayerPrefs.Save();
        Debug.Log("Loading level: " + currentSelectedLevelTitle);
        Debug.Log("Checking prefs: " + PlayerPrefs.GetString("LevelCreatorPath"));


        SceneManager.LoadScene("LevelCreator");


    }

    public void DeleteLevel()
    {
        // Remove the level name from the list and update the data file.
        LevelDataConverter.instance.DeleteLevelByName(currentSelectedLevelTitle);
        Initialize();
        selectedLevelDisplay.SetActive(false);
        currentSelectedLevelTitle = string.Empty;
    }


    public void SetInputFieldActive(bool active)
    {
        inputField.SetActive(active);
    }
}