using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PopUpsDisplay : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private GameObject frozen;
    private PlayerMovement playerMove;
    [SerializeField] private float lerpTime = 1f; // Time taken for the lerp
    private float currentLerpTime = 0f; // Current time of the lerp
    private GameObject player;
    [SerializeField] private Image gameoverImage; // Reference to the gameover image component
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private float positionArrivalThreshold = 1f; // Threshold to determine when the image has reached the target position

    private PlayerManager playerMan;
   

    private ScoreManager scoreMan;

    void Start()
    {
       
        gameoverImage.gameObject.SetActive(false); // Disable the gameover image at the start
        frozen.SetActive(false);
        initialPosition = gameoverImage.transform.position;
    }

    void Update()
    {
       
    //    Frozen();
       

    }

     void OnEnable()
    {
        ID.globalEvents.Frozen += Frozen;

    }
    void OnDisable()
    {
        ID.globalEvents.Frozen -= Frozen;
    }

     void OnDestroy()
    {
        
    }
   public void GameOver()
{
    // Set the initial position above the screen
    initialPosition = new Vector2(initialPosition.x, Screen.height * 1.5f);
    gameoverImage.transform.position = initialPosition;
    gameoverImage.gameObject.SetActive(true); // Enable the gameover image
    targetPosition = new Vector2(Screen.width / 2, Screen.height / 2); // Center of the screen

    // Reset the lerp time
    currentLerpTime = 0f;

    // Start the coroutine to handle the lerp
    StartCoroutine(MoveGameOverImage());
}

private IEnumerator MoveGameOverImage()
{
    while (currentLerpTime < lerpTime)
    {
        currentLerpTime += Time.deltaTime;
        float perc = currentLerpTime / lerpTime;
         float smoothT = Mathf.SmoothStep(0, 1, perc);

        gameoverImage.transform.position = Vector2.Lerp(initialPosition, targetPosition, smoothT);

        yield return null;
    }
}

private void Frozen()
{ 
    frozen.SetActive(true);
    StartCoroutine(FrozenTime());
}

private IEnumerator FrozenTime()
{
    yield return new WaitForSeconds(1.5f);
    frozen.SetActive(false);
}


      
    

}
