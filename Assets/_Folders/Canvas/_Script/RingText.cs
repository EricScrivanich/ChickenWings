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
    [SerializeField] private TextMeshProUGUI numberText;

    [SerializeField] private LevelManagerID LvlID;

    // Start is called before the first frame update
    private void UpdateRingPanel(int number)
    {
        numberText.text = number.ToString() + " / " + LvlID.ringsNeeded.ToString();

    }
    private void UpdateBarnPanel(int number)
    {
        currentBarns++;

        numberText.text = currentBarns.ToString() + " / " + LvlID.barnsNeeded.ToString();

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

    }
}
