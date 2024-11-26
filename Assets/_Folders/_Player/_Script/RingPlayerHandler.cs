using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPlayerHandler : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private LevelManagerID lvlID;

    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private Transform Mana;
    [SerializeField] private List<GameObject> Lives;

    [SerializeField] private int BucketDifferntObjective;

    public RingID RingRed;
    public RingID RingPink;
    public RingID RingGold;
    public RingID RingPurple;
    private CanvasScreenPositions positionScript;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("CanvasScreen") != null)
        {

            positionScript = GameObject.Find("CanvasScreen").GetComponent<CanvasScreenPositions>();


        }
        else
        {
            Debug.Log("No Canvas Screen Display Gameobject Found In RingPlayerHandler script");
        }

    }



    private void AddScore()
    {


        ID.AddScore(2);

    }




    private void BucketCompletion(int index)
    {
        switch (index)
        {
            case 0:


                break;
            case 1:
                if (ID.Lives < 3)
                {
                    RingPink.GetBall(transform.position, null, positionScript.ReturnPinkPosition());

                }

                break;

            case 2:

                Vector2 pos = positionScript.ReturnGoldPosition();

                if (pos == Vector2.zero) return;

                RingGold.GetBall(transform.position, null, pos);

                break;

            case 3:

                RingPurple.GetBall(transform.position, null, Mana.position);

                break;

            default:
                break;
        }

    }
    private void GoldFinish()
    {

        if (BucketDifferntObjective == 2 && lvlID != null)
        {
            Invoke("DelayBeforeGoldAdded", .25f);
            return;

        }
        ID.AddScore(Mathf.CeilToInt(RingGold.CorrectRing * 1.5f));
    }

    private void PinkFinish()
    {

        ID.Lives += 1;
        AudioManager.instance.PlayBucketSuccessSound();


    }

    private void DelayBeforeGoldAdded()
    {
        lvlID.outputEvent.addBucketPass?.Invoke();
    }

    private void PurpleFinish()
    {

    }

    private void RedFinish()
    {
        ID.events.LoseLife?.Invoke();
    }
    private void LoseLife(Vector2 startPos)
    {
        RingRed.GetBall(startPos, this.gameObject);


    }

    private Vector2 ConvertUIToWorldPosition(GameObject uiElement)
    {
        // Get the RectTransform component of the UI element
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

        // Convert the UI element's screen position to world space
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(rectTransform.position);

        return worldPosition;
    }
    private void Test(Vector2 startPos)
    {


    }
    private void GainLife()
    {

    }

    private void OnEnable()
    {
        RingGold.ringEvent.OnCheckOrder += AddScore;

        RingRed.ringEvent.OnGetBall += LoseLife;

        ID.globalEvents.OnBucketExplosion += BucketCompletion;
        RingPink.ringEvent.OnBallFinished += PinkFinish;
        RingGold.ringEvent.OnBallFinished += GoldFinish;
        RingRed.ringEvent.OnBallFinished += RedFinish;
        RingPurple.ringEvent.OnBallFinished += PurpleFinish;




    }
    private void OnDisable()
    {
        RingPink.ringEvent.OnBallFinished -= PinkFinish;
        RingGold.ringEvent.OnBallFinished -= GoldFinish;
        RingRed.ringEvent.OnBallFinished -= RedFinish;
        RingPurple.ringEvent.OnBallFinished -= PurpleFinish;



        RingGold.ringEvent.OnCheckOrder -= AddScore;


        RingRed.ringEvent.OnGetBall -= LoseLife;



        ID.globalEvents.OnBucketExplosion -= BucketCompletion;


    }
}
