using UnityEngine;
using DG.Tweening;

public class MenuToLevelPickerTransition : MonoBehaviour
{
    [SerializeField] private float delayToMove;
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private GameObject[] levelPickerObjects;
    [SerializeField] private float moveXAmount;
    [SerializeField] private float moveXDuration;
    [SerializeField] private float moveYDuration;
    [SerializeField] private Ease cameraEase;

    [SerializeField] private Transform[] moveYObjects;
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

    public void TransitionToLevelPicker()
    {


        Invoke("Logic", delayToMove);


    }

    public void Logic()
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
    }

    // Update is called once per frame

}
