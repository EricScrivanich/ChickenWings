using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPlayerHandler : MonoBehaviour
{
    public PlayerID ID;

    [SerializeField] private GameObject scoreText;
    [SerializeField] private Transform Mana;
    [SerializeField] private List<GameObject> Lives;

    public RingID RingRed;
    public RingID RingPink;
    public RingID RingGold;
    public RingID RingPurple;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void AddScore()
    {

        ID.AddScore(3);

    }

    private void AddMana()
    {


        ID.CurrentMana += 40;
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
                    RingPink.GetBall(transform.position, null, Lives[ID.Lives - 1].transform.position);

                }

                break;

            case 2:

                RingGold.GetBall(transform.position, null, scoreText.transform.position);

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
        ID.AddScore(Mathf.CeilToInt(RingGold.CorrectRing * 2) + 3);
    }

    private void PinkFinish()
    {

        ID.Lives += 1;


    }

    private void PurpleFinish()
    {
        ID.CurrentMana += (Mathf.CeilToInt(RingPurple.CorrectRing * 10));
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
        RingPurple.ringEvent.OnCheckOrder += AddMana;
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
        RingPurple.ringEvent.OnCheckOrder -= AddMana;

        RingRed.ringEvent.OnGetBall -= LoseLife;



        ID.globalEvents.OnBucketExplosion -= BucketCompletion;


    }
}
