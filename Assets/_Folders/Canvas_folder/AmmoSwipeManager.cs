using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmmoSwipeManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // private EggAmmoDisplay parentScript;
    [SerializeField] private PlayerID player;

    private bool isRotating;
    private bool isPressed;
    private float lastPointerPosition;
    private float dragThreshold = 60;
    private bool hasReleased = true;
    // Start is called before the first frame update
    void Start()
    {
        // parentScript = GetComponentInParent<EggAmmoDisplay>();

    }

    // Update is called once per frame
    public void OnDrag(PointerEventData eventData)
    {

        // if (!isRotating && isPressed)
        // {
        //     float dragDistanceX = eventData.position.x - lastPointerPosition;

        //     if (dragDistanceX < -dragThreshold)
        //     {
        //         // parentScript.RotateChamber(false);
        //         player.UiEvents.OnSwitchWeapon?.Invoke(1);
        //         isRotating = true;
        //         // Invoke("SetRotatingFalse", .4f);
        //     }
        //     else if (dragDistanceX > dragThreshold)
        //     {
        //         // parentScript.RotateChamber(true);
        //         player.UiEvents.OnSwitchWeapon?.Invoke(-1);

        //         isRotating = true;
        //         // Invoke("SetRotatingFalse", .4f);
        //     }


        // }

        if (isPressed && hasReleased)
        {
            float dragDistanceX = eventData.position.x - lastPointerPosition;

            if (dragDistanceX < -dragThreshold)
            {
                // parentScript.RotateChamber(false);
                player.UiEvents.OnPressAmmoSideButton?.Invoke(2);
                hasReleased = false;
                player.UiEvents.OnSwitchWeapon?.Invoke(1, -1);
                isRotating = true;

                // Invoke("SetRotatingFalse", .4f);
            }
            else if (dragDistanceX > dragThreshold)
            {
                // parentScript.RotateChamber(true);
                hasReleased = false;
                player.UiEvents.OnPressAmmoSideButton?.Invoke(2);

                player.UiEvents.OnSwitchWeapon?.Invoke(-1, -1);

                isRotating = true;

                // Invoke("SetRotatingFalse", .4f);
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
        // player.UiEvents.OnPressAmmoSideButton?.Invoke(2);

        if (hasReleased) player.UiEvents.OnSwitchWeapon?.Invoke(-2, -1);


        isPressed = false;
        hasReleased = true;
        // player.UiEvents.OnClickAmmoSwitch?.Invoke(false);

        // parentScript.HighlightButtons(false);



        // Enable further rotation after release, using the last known angular velocity
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        player.UiEvents.OnPressAmmoSideButton?.Invoke(1);


        isPressed = true;
        lastPointerPosition = eventData.position.x;
        // player.UiEvents.OnClickAmmoSwitch?.Invoke(true);
        // parentScript.HighlightButtons(true);




        // Enable further rotation after release, using the last known angular velocity
    }
}
