using UnityEngine;

public class FolderManager : MonoBehaviour
{
    public static FolderManager instance;

    [SerializeField] private FolderButton[] folderButtons;
    [SerializeField] private GameObject[] folders;

    private int currentFolder = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    public void SelectFolder(int type)
    {
        folderButtons[currentFolder].Unselect();

        folders[currentFolder].SetActive(false);

        folders[type].SetActive(true);
        currentFolder = type;

    }
}
