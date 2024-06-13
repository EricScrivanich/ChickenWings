using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingParent : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;
    public RingID ID;
    private int ringAmount;
    public int correctRing { get; private set; }
    private List<TutorialRingMovement> rings;

    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        rings = new List<TutorialRingMovement>();
        ringAmount = 0;
        foreach (Transform child in transform)
        {
            ringAmount++;
            rings.Add(child.gameObject.GetComponent<TutorialRingMovement>());

        }
        // HighlightCorrectRing(correctRing);


    }

    public bool CheckOrder(int order)
    {
        bool isCorrect = order == correctRing;
        if (isCorrect)
        {
            correctRing++;
            HighlightCorrectRing(correctRing);
            if (order == ringAmount)
            {
                Completed();
                ID.ringEvent.tutorialRingPass?.Invoke();
            }
        }

        return isCorrect;



    }
    private void Completed()
    {
        LvlID.inputEvent.RingParentPass?.Invoke(0);

    }
    private void HighlightCorrectRing(int correctInt)
    {
        foreach (var ring in rings)
        {
            ring.CheckForHighlight(correctInt);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

    }
    private void OnEnable()
    {
        correctRing = 1;



    }


}
