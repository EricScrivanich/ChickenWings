using UnityEngine;
using DG.Tweening;
public class RecordableObjectPlacerPositionType : MonoBehaviour
{

    // [SerializeField] private GameObject prefab;
    // public short ID;

    // private RecordedDataStructTweensDynamic positionData;
    // private RecordedDataStructTweensDynamic rotationData;
    // private RecordedDataStructTweensDynamic timerData;

    // private Ease[] eases = new Ease[] { Ease.Linear, Ease.InSine, Ease.OutSine, Ease.InOutSine };

    // public enum EditableData
    // {
    //     Position,
    //     Rotation,
    //     Timer

    // }

    // public EditableData[] editableData;


    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // public void CreateNew()
    // {
    //     for (int i = 0; i < editableData.Length; i++)
    //     {
    //         switch (editableData[i])
    //         {
    //             case EditableData.Position:
    //                 positionData = new RecordedDataStructTweensDynamic(ID, 0);
    //                 positionData.postions.Add(transform.position);

    //                 break;
    //             case EditableData.Rotation:
    //                 rotationData = new RecordedDataStructTweensDynamic(ID, 1);

    //                 break;
    //             case EditableData.Timer:
    //                 timerData = new RecordedDataStructTweensDynamic(ID, 2);
    //                 break;

    //         }
    //     }
    // }
    // public void UpdateTimeStep(ushort timestep)
    // {
    //     for (int i = 0; i < editableData.Length; i++)
    //     {
    //         switch (editableData[i])
    //         {
    //             case EditableData.Position:
    //                 if (positionData.startingSteps == null || positionData.startingSteps.Count <= 1)
    //                 {
    //                     return;
    //                 }


    //                 for (int j = 0; j < positionData.startingSteps.Count - 1; j++)
    //                 {
    //                     if (timestep > positionData.startingSteps[j] && timestep <= positionData.endingSteps[j])
    //                     {
    //                         // float startTime = positionData.startingSteps[j] * LevelRecordManager.TimePerStep;
    //                         // float endTime = positionData.endingSteps[j] * LevelRecordManager.TimePerStep;
    //                         // float percent = 
    //                         // Vector2 val = DOVirtual.EasedValue(positionData.postions[j], positionData.postions[j + 1], (float)(timestep - positionData.appraochTimes[j]) / (float)(positionData.appraochTimes[j + 1] - positionData.appraochTimes[j]), eases[positionData.easeTypes[j]]);
    //                         // transform.position = val;
    //                         // break;


    //                     }



    //                 }
    //                 break;
    //             // case EditableData.Rotation:
    //             //     rotationData.UpdateTimeStep(timestep);
    //             //     break;
    //             // case EditableData.Timer:
    //             //     timerData.UpdateTimeStep(timestep);
    //             //     break;

    //         }
    //     }

    // }

    // public void UpdateBasePosition(Vector2 pos)
    // {


    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
