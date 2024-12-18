using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysSignParent : MonoBehaviour
{
    [SerializeField] private List<SidewaysSignMovement> signList;
    [SerializeField] private List<TipSignMovement> tipSignList;

    private List<Vector2> OgPositions;
    private RectTransform StartingSignRect;
    private Vector2 StartingSignRectPosition;
    private int currentIndex;
    // Start is called before the first frame update
    void Start()
    {


    }
    private void Awake()
    {

        currentIndex = 0;
        // OgPositions = new List<Vector2>();
        // foreach (var obj in signList)
        // {
        //     Vector2 pos = obj.GetComponent<RectTransform>().position;
        //     OgPositions.Add(pos);

        // }
    }

    public void SwitchSign(bool isNext)
    {
        // Debug.Log("Side Index: " + currentIndex);
        HapticFeedbackManager.instance.PressUIButton();

        if (isNext)
        {
            signList[currentIndex].AnimateSign(true, false);
            int tipSignPrev = signList[currentIndex].TipSignIndex;
            if (tipSignPrev > 0) tipSignList[tipSignPrev - 1].Retract();

            currentIndex++;
            if (currentIndex >= signList.Count)
            {
                currentIndex--;
                signList[currentIndex].RetractToNextUI(true);
                return;
            }

            signList[currentIndex].AnimateSign(true, true);
            int tipSignNext = signList[currentIndex].TipSignIndex;
            if (tipSignNext > 0) tipSignList[tipSignNext - 1].DropSignTween();
        }

        else
        {
            signList[currentIndex].AnimateSign(false, false);
            int tipSignPrev = signList[currentIndex].TipSignIndex;
            if (tipSignPrev > 0) tipSignList[tipSignPrev - 1].Retract();

            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
                signList[0].RetractToNextUI(false);
                return;

            }
            signList[currentIndex].AnimateSign(false, true);
            int tipSignNext = signList[currentIndex].TipSignIndex;
            if (tipSignNext > 0) tipSignList[tipSignNext - 1].DropSignTween();
        }
    }

    private void OnEnable()
    {
        signList[currentIndex].DropSign();
        int tipSignNext = signList[currentIndex].TipSignIndex;
        if (tipSignNext > 0) tipSignList[tipSignNext - 1].DropSignTween();

    }



    private void OnDisable()
    {
        // for (int i = 0; i < signList.Count; i++)
        // {
        //     Vector2 pos = signList[i].GetComponent<RectTransform>().position = OgPositions[i];


        // }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
