using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PanelText : MonoBehaviour
{
    public PlayerID player;
    private int currentBarns;
    [SerializeField] private bool isRing;
    [SerializeField] private bool isBarn;
    [SerializeField] private bool isPig;
    [SerializeField] private bool isBucket;
       [SerializeField] private TextMeshProUGUI numberText;

    [SerializeField] private LevelManagerID LvlID;

    // Start is called before the first frame update
    private void UpdateRingPanel(int number)
    {
        numberText.text = number.ToString() + " / " + LvlID.ringsNeeded.ToString();

    }
    private void UpdateBarnPanel(int number)
    {
        if (currentBarns >= LvlID.barnsNeeded)
            return;
        currentBarns++;


        numberText.text = currentBarns.ToString() + " / " + LvlID.barnsNeeded.ToString();

    }

    private void UpdateBucketPanel(int number)
    {
        numberText.text = number.ToString() + " / " + LvlID.bucketsNeeded.ToString();
    }

    private void UpdatePigPannel(int number, int needed)
    {
        numberText.text = number.ToString() + " / " + needed.ToString();

    }



    private void OnEnable()
    {
        if (isRing)
        {
            LvlID.outputEvent.RingParentPass += UpdateRingPanel;
            numberText.text = "0" + " / " + LvlID.ringsNeeded.ToString();
        }
        else if (isBarn)
        {
            player.globalEvents.OnAddScore += UpdateBarnPanel;
            currentBarns = 0;
            numberText.text = "0" + " / " + LvlID.barnsNeeded.ToString();

        }
        else if (isBucket)
        {
            LvlID.outputEvent.setBucketPass += UpdateBucketPanel;
            numberText.text = "0" + " / " + LvlID.bucketsNeeded.ToString();

        }
        else if (isPig)
        {
            LvlID.outputEvent.killedPig += UpdatePigPannel;


        }


    }
    private void OnDisable()
    {
        if (isRing)
        {
            LvlID.outputEvent.RingParentPass -= UpdateRingPanel;
        }
        else if (isBarn)
        {
            player.globalEvents.OnAddScore -= UpdateBarnPanel;

        }
        else if (isBucket)
        {
            LvlID.outputEvent.setBucketPass -= UpdateBucketPanel;

        }
        else if (isPig)
        {
            LvlID.outputEvent.killedPig -= UpdatePigPannel;


        }

    }
}
