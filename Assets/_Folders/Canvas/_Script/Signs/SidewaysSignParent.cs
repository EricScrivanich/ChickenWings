using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysSignParent : MonoBehaviour
{
    [SerializeField] private List<SidewaysSignMovement> signList;

    private List<Vector2> OgPositions;
    private RectTransform StartingSignRect;
    private Vector2 StartingSignRectPosition;
    private int currentIndex;
    // Start is called before the first frame update
    void Start()
    {
       
        
    }
    private void Awake() {

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
        if (isNext)
        {
            signList[currentIndex].AnimateSign(true, false);

            currentIndex++;
            if (currentIndex >= signList.Count)
            {
                currentIndex--;
                signList[currentIndex].RetractToNextUI(false);
                return;
            }
           
            signList[currentIndex].AnimateSign(true,true);
        }

        else
        {
            signList[currentIndex].AnimateSign(false, false);

            currentIndex--;
            if (currentIndex < 0)
            {
                signList[0].RetractToNextUI(false);
                return;

            }
            signList[currentIndex].AnimateSign(false, true);
        }
    }

    private void OnEnable() {
        signList[currentIndex].DropSign();

    }

    public void NextSign()
    {

    }

    private void OnDisable() {
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
