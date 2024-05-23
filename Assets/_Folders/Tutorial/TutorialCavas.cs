using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



public class TutorialCavas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ringText;
    [SerializeField] private Image target;
    public bool tweenFinished;
    public int ringsNeeded;
    public CanvasGroup panelCanvasGroup;  // Reference to the CanvasGroup of the panel for fading
    public RectTransform panelRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform.anchoredPosition = new Vector2(0, 800);  // Start position off-screen


    }

    public void AnimateStats()
    {



        // Move panel into view
        panelRectTransform.DOAnchorPos(new Vector2(0, 425), 1.5f);  // 
    }
    public void UpdateRings(int passed, int needed)
    {

        if (ringText != null)
        {
            ringText.text = passed.ToString() + " / " + needed.ToString();
        }
    }

    // Update is called once per frame

}
