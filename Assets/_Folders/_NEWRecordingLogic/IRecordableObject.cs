using UnityEngine;

public interface IRecordableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RecordedDataStruct RecordData();
    void RetrieveRecordedData(RecordedDataStruct data);

    void ShowPath(bool isSelected);


}
