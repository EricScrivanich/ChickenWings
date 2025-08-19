using UnityEngine;

public class AmmoEggState : AmmoBaseState
{
    private int ammoType = 0;

    public override void EnterState(PlayerStateManager player, int direction)
    {
        Debug.LogError("In Egg State");

        player.ID.UiEvents.OnSwitchDisplayedWeapon?.Invoke(0, player.ID.Ammo, direction);

        // if (player.ID.Ammo <= 0)
        //     player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);





    }

    public override void ExitState(PlayerStateManager player)
    {

    }

    public override void PressButton(PlayerStateManager player, Vector2 startPos)
    {


        if (player.ID.Ammo > 0)
        {


            AudioManager.instance.PlayEggDrop();
            HapticFeedbackManager.instance.PlayerButtonPress();
            
            player.GetEgg();

            player.ID.Ammo -= 1;
            player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.Ammo);
            if (player.ID.Ammo <= 0)
            {
                player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);
                player.ID.UiEvents.OnPressWeaponButton?.Invoke(0);
                // player.ID.CheckAmmosOnZero(-1);
            }
            else
                player.ID.UiEvents.OnPressWeaponButton?.Invoke(0);



        }
        else
        {
            player.ID.UiEvents.OnSetAmmoZero?.Invoke(true);
            // player.SetIfWeaponButtonPressed(false);

        }
    }

    public override void ReleaseButton(PlayerStateManager player)
    {

    }

    public override void CollectAmmo(PlayerStateManager player, int type)
    {
        if (type == ammoType) player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.Ammo);
    }
    public override void SwipeButton(PlayerStateManager player, Vector2 direction)
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
