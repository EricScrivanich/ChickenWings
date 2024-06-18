using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RingText : MonoBehaviour
{
    [SerializeField] private GameObject RingPanel;
    [SerializeField] private TextMeshProUGUI RingNumber;

    [SerializeField] private LevelManagerID LvlID;

    // Start is called before the first frame update
    private void UpdateRingPanel(int number)
    {
        RingNumber.text = number.ToString() + " / " + LvlID.ringsNeeded.ToString();

    }



    private void OnEnable()
    {
        LvlID.outputEvent.RingParentPass += UpdateRingPanel;

        RingNumber.text = "0" + " / " + LvlID.ringsNeeded.ToString();

    }
    private void OnDisable()
    {
        LvlID.outputEvent.RingParentPass -= UpdateRingPanel;
    }
}
