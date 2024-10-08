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

    [SerializeField] private float moveAmountForRecording;

    [SerializeField] private RectTransform[] moveRects;

    [SerializeField] private bool moveRectsForRecording;
    // Start is called before the first frame update


    void Awake()
    {
        if (moveRectsForRecording)
        {
            foreach (var r in moveRects)
            {
                r.anchoredPosition = new Vector2(r.anchoredPosition.x - moveAmountForRecording, r.anchoredPosition.y);
            }

            if (GameObject.Find("CanvasScreen") != null)
            {
                RectTransform lives = GameObject.Find("Lives3").GetComponent<RectTransform>();

                if (lives != null)
                    lives.anchoredPosition = new Vector2(lives.anchoredPosition.x - moveAmountForRecording, lives.anchoredPosition.y);
            }
        }
    }
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
