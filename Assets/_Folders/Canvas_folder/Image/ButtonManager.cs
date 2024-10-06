using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [ExposedScriptableObject]
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private Material yer;
    [SerializeField] private Image[] outlines;
    [SerializeField] private Image[] fills;
    [SerializeField] private Image[] fullFills;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Image img in outlines)
        {
            img.color = colorSO.OutLineColor;
            // img.color = Color.white;
            // img.material = yer;


        }

        foreach (Image img in fills)
        {
            img.color = colorSO.normalButtonColor;
            // img.material = yer;
        }


        foreach (Image img in fullFills)
        {
            img.color = colorSO.normalButtonColorFull;
            // img.material = yer;
        }

    }

}
