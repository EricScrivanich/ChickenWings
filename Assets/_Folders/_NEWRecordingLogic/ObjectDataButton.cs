using UnityEngine;
using TMPro;

public class ObjectDataButton : MonoBehaviour
{
    public string Type { get; private set; }
    public TextMeshProUGUI text;

    [SerializeField]
    private
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void SetType(string type)
    {
        Type = type;
    }

    // void OnPress()
    // {
    //     LevelRecordManager.instance.SetEditedData(Type);
    // }

    // Update is called once per frame

}
