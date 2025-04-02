using UnityEngine;
using DG.Tweening;
using System.Collections;

public class AmmoShotgunState : AmmoBaseState
{
    private int ammoType = 1;
    private float shotgunRotationTarget;
    private float shotgunRotationSpeed;
    private float slowTimeDuration = .15f;
    private float speedTimeDuration = .4f;
    private float targetTimeScale = .48f;
    private float maxHoldTime = 1.2f;
    private bool usingChainedAmmo;
    private bool isPressed = false;
    private bool inState;
    private bool usingTimer;
    private Coroutine timerForShotgunRotation;

    private bool rotatingToTarget;

    private Vector2 startPos;

    private int dragThreshold = 200;
    private float rotationTarget;


    public override void EnterState(PlayerStateManager player, int direction)
    {
        Debug.LogError("In SHotgun State");
        rotationTarget = 0;
        inState = true;
        rotatingToTarget = false;

        isPressed = false;
        usingChainedAmmo = false;


        player.ID.UiEvents.OnSwitchDisplayedWeapon?.Invoke(1, player.ID.ShotgunAmmo, direction);
        if (player.ID.ShotgunAmmo <= 0)
            player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);


        player.shotgunEquipped = true;
        player.movingJoystick = false;


        // anim.SetTrigger("Equip");
        player.SetShotgunRotationAndActive(true, -45);
        player.anim.SetTrigger(player.EquipShotgunTrigger);



        shotgunRotationTarget = -20;
        player.RotateShotgun(true);
        // player.ShotgunNormalRotationRoutine = player.StartCoroutine(player.RotateShotgun());






    }

    public void StopChainedAmmo(PlayerStateManager player, bool stopAll)
    {
        if (usingChainedAmmo && inState)
        {
            usingChainedAmmo = false;
            player.ID.UiEvents.OnUseChainedAmmo?.Invoke(false);
            player.ID.ChainedShotgunAmmo = -1;



            if (player.ID.ShotgunAmmo <= 0)
            {
                player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);


                player.ID.UiEvents.ReleaseScope?.Invoke(false);
                // player.ID.CheckAmmosOnZero(-1);

            }

            player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ShotgunAmmo);

            if (isPressed && stopAll)
            {
                player.ID.events.EnableButtons?.Invoke(true);
                player.ID.UiEvents.OnUseJoystick?.Invoke(false);

                player.anim.SetBool(player.AimShotgunBool, false);

                player.SetTimeScale(false, speedTimeDuration);

            }

        }
    }



    public override void ExitState(PlayerStateManager player)
    {
        player.movingJoystick = false;
        inState = false; ;
        player.SetShotgunRotationAndActive(false, -45);

        player.RotateShotgun(false);

        // if (player.ShotgunNormalRotationRoutine != null)

        //     player.StopCoroutine(player.ShotgunNormalRotationRoutine);

        // if (player.ShotgunAimingRotationRoutine != null)
        //     player.StopCoroutine(player.ShotgunAimingRotationRoutine);

        player.anim.SetTrigger(player.UnEquipShotgunTrigger);

        player.anim.SetBool(player.AimShotgunBool, false);
        player.shotgunEquipped = false;
        player.ID.ChainedShotgunAmmo = -1;


    }





    public override void PressButton(PlayerStateManager player, Vector2 startPos)
    {

        if (player.ID.ShotgunAmmo > 0 || (usingChainedAmmo && player.ID.ChainedShotgunAmmo > 0))
        {
            rotatingToTarget = false;

            HapticFeedbackManager.instance.PressShotgunButton();
            player.shotgunRotationTarget = 0;
            Debug.LogError("Pressed");
            player.maxFallSpeed = -8.5f;
            isPressed = true;
            // player.HandleShotgun(true);
            player.ID.canUseJoystick = true;
            player.ID.events.EnableButtons?.Invoke(false);
            // player.ID.UiEvents.OnUseJoystick?.Invoke(true);
            player.anim.SetBool(player.AimShotgunBool, true);
            player.SetTimeScale(true, slowTimeDuration, targetTimeScale, maxHoldTime);
            player.ID.UiEvents.OnPressWeaponButton?.Invoke(maxHoldTime);

        }
        else
        {
            isPressed = false;
            player.ID.UiEvents.OnSetAmmoZero?.Invoke(true);
            // player.SetIfWeaponButtonPressed(false);
            return;


        }






        // ReadyOrPlayPressSeq(false);

        // manager.PressScope(manager.player.maxShotgunHoldTime);
        // HapticFeedbackManager.instance.PressShotgunButton();

        // PressSeq.Append(scopeRect.DOScale(BoundariesManager.vectorThree1 * scaleAmountOnPress, buttonPressTweenDuration));
        // PressSeq.Join(currentEgg.DOScale(BoundariesManager.vectorThree1 * (scaleAmountOnPress + .05f), buttonPressTweenDuration).SetEase(Ease.OutSine));
        // PressSeq.Join(currentEgg.DOLocalMoveY(originalEggY + addedYOnPress + 5, buttonPressTweenDuration));
        // PressSeq.Join(scopeRect.DOLocalMoveY(originalYPos + addedYOnPress, buttonPressTweenDuration));
        // PressSeq.Join(scopeFill.DOColor(colorSO.highlightButtonColor, buttonPressTweenDuration));



    }
    private IEnumerator Timer(PlayerStateManager player)
    {
        yield return new WaitForSeconds(.2f);
        player.GetShell();

        AudioManager.instance.PlayShoutgunNoise(1);


    }

    public void HitRotationTarget(PlayerStateManager player)
    {
        isPressed = false;
        // if (player.ID.ShotgunAmmo > 0)
        // {
        //     usingChainedAmmo = false;

        // }
        HapticFeedbackManager.instance.ReleaseShotgunButton();

        player.StartCoroutine(Timer(player));

        player.GetShotgunBlast(usingChainedAmmo);
        player.shotgunRotationTarget = 45;
        player.ID.events.EnableButtons?.Invoke(true);
        // player.ID.UiEvents.OnUseJoystick?.Invoke(false);

        player.anim.SetBool(player.AimShotgunBool, false);

        player.SetTimeScale(false, speedTimeDuration);
        if (player.ID.ShotgunAmmo > 0)
        {
            player.ID.ShotgunAmmo--;

            if (player.ID.ShotgunAmmo <= 0 && !usingChainedAmmo)
            {
                usingChainedAmmo = true;
                player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ChainedShotgunAmmo);
                player.ID.UiEvents.OnUseChainedAmmo?.Invoke(true);
            }
            else
            {
                player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ShotgunAmmo);
            }
        }



        else if (usingChainedAmmo && player.ID.ChainedShotgunAmmo > 0)
        {
            player.ID.ChainedShotgunAmmo--;
            player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ChainedShotgunAmmo);

            if (player.ID.ChainedShotgunAmmo <= 0)
            {
                StopChainedAmmo(player, false);
                return;

            }


        }

        player.ID.UiEvents.ReleaseScope?.Invoke(false);

    }



    public override void ReleaseButton(PlayerStateManager player)
    {
        player.ID.canUseJoystick = false;


        if (!isPressed || rotatingToTarget) return;
        isPressed = false;
        // if (player.ID.ShotgunAmmo > 0)
        // {
        //     usingChainedAmmo = false;

        // }
        HapticFeedbackManager.instance.ReleaseShotgunButton();

        player.StartCoroutine(Timer(player));

        player.GetShotgunBlast(usingChainedAmmo);
        player.shotgunRotationTarget = 45;
        player.ID.events.EnableButtons?.Invoke(true);
        // player.ID.UiEvents.OnUseJoystick?.Invoke(false);

        player.anim.SetBool(player.AimShotgunBool, false);

        player.SetTimeScale(false, speedTimeDuration);
        if (player.ID.ShotgunAmmo > 0)
        {
            player.ID.ShotgunAmmo--;

            if (player.ID.ShotgunAmmo <= 0 && !usingChainedAmmo)
            {
                usingChainedAmmo = true;
                player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ChainedShotgunAmmo);
                player.ID.UiEvents.OnUseChainedAmmo?.Invoke(true);
            }
            else
            {
                player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ShotgunAmmo);
            }
        }



        else if (usingChainedAmmo && player.ID.ChainedShotgunAmmo > 0)
        {
            player.ID.ChainedShotgunAmmo--;
            player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ChainedShotgunAmmo);

            if (player.ID.ChainedShotgunAmmo <= 0)
            {
                StopChainedAmmo(player, false);
                return;

            }


        }

        player.ID.UiEvents.ReleaseScope?.Invoke(false);

        // player.HandleShotgun(false);



        // manager.ResetScope(false);
        // HapticFeedbackManager.instance.ReleaseShotgunButton();

    }

    public override void CollectAmmo(PlayerStateManager player, int type)
    {
        if (type == ammoType)
        {
            if (usingChainedAmmo)
                StopChainedAmmo(player, false);
            else
                player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.ShotgunAmmo);
        }
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
                rotationTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (rotationTarget < 0) rotationTarget += 360;
                rotatingToTarget = true;
                player.RotateToTargetFromWeaponState(rotationTarget, 700);
                isPressed = false;

            }

        }




    }
}
