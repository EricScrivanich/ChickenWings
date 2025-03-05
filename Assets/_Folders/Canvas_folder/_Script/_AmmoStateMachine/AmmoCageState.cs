using UnityEngine;

public class AmmoCageState : AmmoBaseState
{
    public override void EnterState(PlayerStateManager player, int direction)
    {
        // player.ID.UiEvents.OnShowCage?.Invoke(true);
        // player.ID.canPressEggButton = true;
        player.ID.UiEvents.OnSwitchDisplayedWeapon(-2, 0, 0);



    }


    public override void ExitState(PlayerStateManager player)
    {
        player.ID.UiEvents.OnShowCage?.Invoke(false);

    }

    public override void PressButton(PlayerStateManager player, Vector2 startPos)
    {

        player.ID.UiEvents.OnSwitchWeapon?.Invoke(0, -3);
        player.ID.globalEvents.OnReleaseCage?.Invoke();

    }

    public override void ReleaseButton(PlayerStateManager player)
    {

    }
    public override void CollectAmmo(PlayerStateManager player, int type)
    {

    }
    public override void SwipeButton(PlayerStateManager player, Vector2 direction)
    {

    }

}