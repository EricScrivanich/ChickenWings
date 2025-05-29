using UnityEngine;

public class EditorMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject listPanel;

    public static EditorMain instance;

    // private void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    //     else
    //     {
    //         Destroy(this);
    //     }

    //     ShowMainPanel();
    // }
    // public void ShowMainPanel()
    // {
    //     mainPanel.SetActive(true);
    //     listPanel.SetActive(false);
    // }
    // public void ShowListPanel(string type)
    // {
    //     mainPanel.SetActive(false);
    //     RecordedDataStructTweensDynamic d = null;
    //     switch (type)
    //     {
    //         case "Rotations":
    //             d = LevelRecordManager.instance.currentSelectedObject.rotationData;
    //             break;
    //         case "Positions":
    //             d = LevelRecordManager.instance.currentSelectedObject.positionData;
    //             break;
    //         case "Timers":

    //             break;
    //     }
    //     listPanel.GetComponent<DynamicValueAdder>().Activate(type, d);
    //     // listPanel.SetActive(true);
    // }
}
