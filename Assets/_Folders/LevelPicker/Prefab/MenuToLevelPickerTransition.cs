using UnityEngine;
using DG.Tweening;

public class MenuToLevelPickerTransition : MonoBehaviour
{
    [SerializeField] private float delayToMove;
    [SerializeField] private float delayToMoveOut;
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private GameObject[] levelPickerObjects;
    [SerializeField] private float moveXAmount;
    [SerializeField] private float moveXDuration;
    [SerializeField] private float moveYDuration;
    [SerializeField] private Ease cameraEase;

    [SerializeField] private Transform[] moveYObjects;
    [SerializeField] private BackgroundObjects backgroundObjects;
    [SerializeField] private float[] moveToY;
    private float[] moveYOriginalY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveYOriginalY = new float[moveYObjects.Length];
        for (int i = 0; i < moveYObjects.Length; i++)
        {
            moveYOriginalY[i] = moveYObjects[i].localPosition.y;
        }


        if (PlayerPrefs.GetString("NextLevel", "Menu") != "Menu")
        {
            Debug.Log("Transition to level picker from menu with next level set to: " + PlayerPrefs.GetString("NextLevel", "Menu"));
            foreach (GameObject obj in levelPickerObjects)
            {
                obj.SetActive(true);

            }
            Camera.main.transform.position = new Vector3(moveXAmount, Camera.main.transform.position.y, Camera.main.transform.position.z);


        }
        else
        {
            foreach (GameObject obj in levelPickerObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void TransitionToLevelPicker(bool fromMenu)
    {
        if (fromMenu)

            Invoke("ShowLevelPicker", delayToMove);
        else
            Invoke("ReturnToMenu", delayToMoveOut);


    }

    public void ReturnToMenu()
    {
        for (int i = 0; i < moveYObjects.Length; i++)
        {
            moveYObjects[i].DOLocalMoveY(moveYOriginalY[i], moveYDuration).SetEase(Ease.Linear);
        }
        Camera.main.transform.DOMoveX(0, moveXDuration).SetEase(cameraEase);
        backgroundObjects.SetTrackingMovement(true);

        backgroundObjects.transform.DOMoveX(0, moveXDuration).SetEase(cameraEase).OnComplete(() =>
        {
            backgroundObjects.SetTrackingMovement(false);

            backgroundObjects.baseSpeedMultiplier = 1f;
            foreach (GameObject obj in levelPickerObjects)
            {
                obj.SetActive(false);

            }
        });
    }

    public void ShowLevelPicker()
    {
        foreach (GameObject obj in levelPickerObjects)
        {
            obj.SetActive(true);

        }
        for (int i = 0; i < moveYObjects.Length; i++)
        {
            moveYObjects[i].DOLocalMoveY(moveToY[i], moveYDuration).SetEase(Ease.Linear);
        }
        Camera.main.transform.DOMoveX(moveXAmount, moveXDuration).SetEase(cameraEase);
        // make a tween that sets background speed to a 5, then on complete sets it back to .7



        // backgroundObjects.baseSpeedMultiplier = 7f;
        backgroundObjects.SetTrackingMovement(true);
        backgroundObjects.transform.DOMoveX(moveXAmount, moveXDuration).SetEase(cameraEase).OnComplete(() =>
        {
            backgroundObjects.SetTrackingMovement(false);

            backgroundObjects.baseSpeedMultiplier = .7f;
        });

        // Update is called once per frame

    }
}
