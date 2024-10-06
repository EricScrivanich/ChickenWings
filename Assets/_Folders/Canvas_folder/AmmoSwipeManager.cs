using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmmoSwipeManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private EggAmmoDisplay parentScript;
    private bool isRotating;
    private bool isPressed;
    private float lastPointerPosition;
    private float dragThreshold = 60;
    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<EggAmmoDisplay>();

    }

    // Update is called once per frame
    public void OnDrag(PointerEventData eventData)
    {
        if (!isRotating && isPressed)
        {
            float dragDistanceX = eventData.position.x - lastPointerPosition;

            if (dragDistanceX < -dragThreshold)
            {
                parentScript.RotateChamber(false);
                isRotating = true;
                Invoke("SetRotatingFalse", .4f);
            }
            else if (dragDistanceX > dragThreshold)
            {
                parentScript.RotateChamber(true);
                isRotating = true;
                Invoke("SetRotatingFalse", .4f);
            }


        }






    }
    private void SetRotatingFalse()
    {
        AudioManager.instance.PlayChamberCock();
        isPressed = false;

        isRotating = false;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        parentScript.HighlightButtons(false);



        // Enable further rotation after release, using the last known angular velocity
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        lastPointerPosition = eventData.position.x;
        parentScript.HighlightButtons(true);




        // Enable further rotation after release, using the last known angular velocity
    }
}
