using UnityEngine;

public class RecordableObjectVarientTypeHandler : MonoBehaviour
{

    [SerializeField] private RecordableObjectPlacer placer;
    [SerializeField] private ushort varientType;

    public void CheckType(ushort type)
    {
        if (varientType != type)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }

    void Awake()
    {
        placer.OnChangeVarientType += CheckType;
    }
    void OnDestroy()
    {
        placer.OnChangeVarientType -= CheckType;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
