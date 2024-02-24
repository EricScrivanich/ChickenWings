using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPlayerHandler : MonoBehaviour
{
    public PlayerID ID;

    [SerializeField] private GameObject scoreText;
    [SerializeField] private Transform livesPanel;
    [SerializeField] private List<GameObject> Lives;
    
    public RingID RingRed;
    public RingID RingPink;
    public RingID RingGold;
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
        ID.Score += 2;

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
                ID.Score += (Mathf.CeilToInt(RingGold.CorrectRing * 1.5f) + 2);
                RingGold.GetBall(transform.position,null,ConvertUIToWorldPosition(scoreText));
                
                break;


            default:
                break;
        }

    }

    private void PinkFinish()
    {
        
            ID.Lives += 1;

        

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
        RingGold.ringEvent.OnPassedCorrectRing += AddScore;
        RingRed.ringEvent.OnGetBall += LoseLife;
        RingGold.ringEvent.OnGetBall += Test;
        ID.globalEvents.OnBucketExplosion += BucketCompletion;
        RingPink.ringEvent.OnBallFinished += PinkFinish;




    }
    private void OnDisable() 
    {
        RingPink.ringEvent.OnBallFinished -= PinkFinish;

        RingGold.ringEvent.OnPassedCorrectRing -= AddScore;
        RingRed.ringEvent.OnGetBall -= LoseLife;
        RingGold.ringEvent.OnGetBall -= Test;


        ID.globalEvents.OnBucketExplosion -= BucketCompletion;


    }
}
