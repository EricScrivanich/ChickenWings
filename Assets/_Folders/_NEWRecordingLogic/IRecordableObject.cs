using UnityEngine;

public interface IRecordableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // void ApplyRecordedData(RecordedDataStruct data);

    void ApplyFloatOneData(DataStructFloatOne data);
    void ApplyFloatTwoData(DataStructFloatTwo data);
    void ApplyFloatThreeData(DataStructFloatThree data);
    void ApplyFloatFourData(DataStructFloatFour data);
    void ApplyFloatFiveData(DataStructFloatFive data);
    void ApplyCustomizedData(RecordedDataStructDynamic data);

    bool ShowLine();

    float TimeAtCreateObject(int index);
    Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset);

    float ReturnPhaseOffset(float x);




}
