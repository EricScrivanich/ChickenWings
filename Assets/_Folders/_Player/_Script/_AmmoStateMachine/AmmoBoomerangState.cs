using UnityEngine;

public class AmmoBoomerangState : AmmoBaseState
{
    private float slowTimeDuration = .15f;
    private float speedTimeDuration = .4f;
    private float targetTimeScale = .48f;
    private float maxHoldTime = 1.2f;
    private int dragThreshold = 250;

    private bool isPressed = false;


    public override void EnterState(PlayerStateManager player, int direction)
    {

        isPressed = false;
        player.ID.UiEvents.OnSwitchDisplayedWeapon?.Invoke(2, 1, direction);
        player.SetCanSwitchAmmo(false);
        player.ID.globalEvents.EquipItem?.Invoke(2, true);
        // player.ID.UiEvents.OnEquipCage?.Invoke(true);

        // if (player.ID.Ammo <= 0)
        //     player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);





    }

    public override void ExitState(PlayerStateManager player)
    {
        // player.ID.UiEvents.OnEquipCage?.Invoke(false);
        player.ID.globalEvents.EquipItem?.Invoke(2, false);


    }

    public override void PressButton(PlayerStateManager player, Vector2 startPos)
    {
        isPressed = true;
        // player.ID.events.EnableButtons?.Invoke(false);
        // player.ID.UiEvents.OnAllowSwipe?.Invoke(true);
        player.SetTimeScale(true, slowTimeDuration, targetTimeScale, maxHoldTime);
        player.ID.UiEvents.OnPressWeaponButton?.Invoke(maxHoldTime);



    }

    public override void ReleaseButton(PlayerStateManager player)
    {
        player.SetCanSwitchAmmo(true);
        if (isPressed)
        {
            //convert rotation of player.transrorm to a direction vector
            Vector2 direction = (player.transform.right).normalized;
            player.ID.globalEvents.ThrowItem?.Invoke(direction, 0);
            player.AdjustForce((-direction * 6) + (player.rb.linearVelocity * .7f));
            isPressed = false;
        }

        // player.ID.events.EnableButtons?.Invoke(true);
        // player.ID.UiEvents.OnAllowSwipe?.Invoke(false);

        player.SetTimeScale(false, speedTimeDuration);

        // player.ID.UiEvents.ReleaseScope?.Invoke(false);
    }

    public override void CollectAmmo(PlayerStateManager player, int type)
    {


    }

    public override void SwipeButton(PlayerStateManager player, Vector2 currentPos)
    {
        if (isPressed)
        {
            float dragAmount = Vector2.Distance(player.centerTouchStartPos, currentPos);

            if (dragAmount >= dragThreshold)
            {
                // rotationTarget = 
                Vector2 direction = (currentPos - player.centerTouchStartPos).normalized;

                player.ID.globalEvents.ThrowItem?.Invoke(direction, 0);
                player.AdjustForce((-direction * 6) + (player.rb.linearVelocity * .7f));

                isPressed = false;

            }
        }

        // player.ID.globalEvents.ThrowItem?.Invoke(direction, 0);
        // ReleaseButton(player);


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}

