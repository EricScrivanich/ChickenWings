using UnityEngine;

public class AmmoHiddenState : AmmoBaseState
{
    public override void EnterState(PlayerStateManager player, int direction)
    {
        player.ID.UiEvents.OnSwitchDisplayedWeapon?.Invoke(-1, 0, direction);
        // player.ID.canPressEggButton = false;
        player.ID.ammosButtonHidden = true;


    }


    public override void ExitState(PlayerStateManager player)
    {
        // player.ID.canPressEggButton = true;

        player.ID.ammosButtonHidden = false;


    }

    public override void PressButton(PlayerStateManager player,Vector2 pos)
    {

    }

    public override void ReleaseButton(PlayerStateManager player)
    {

    }
    public override void CollectAmmo(PlayerStateManager player, int type)
    {

    }
    public override void SwipeButton(PlayerStateManager player, Vector2 direction, bool isJoystick = false)
    {

    }

}