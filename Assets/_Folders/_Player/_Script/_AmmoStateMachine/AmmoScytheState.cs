using System;
using UnityEngine;

public class AmmoScytheState : AmmoBaseState
{
    private int ammoType = 0;
    private Vector2 scytheSwipeCenterPos;
    private Vector2 startPoint;
    private bool sentSwipeData;
    private int dragThreshold = 135;
    private int angleSwipeMinDiff = 20;


    public void SetCenterPos(Vector2 pos)
    {

        scytheSwipeCenterPos = pos;
        Debug.Log("Setting Scythe pos");
    }
    public override void EnterState(PlayerStateManager player, int direction)
    {
        Debug.LogError("In Egg State");

        player.ID.UiEvents.OnSwitchDisplayedWeapon?.Invoke(3, 1, direction);
        player.ID.UiEvents.OnShowSyctheLine?.Invoke(true);

        // if (player.ID.Ammo <= 0)
        //     player.ID.UiEvents.OnSetAmmoZero?.Invoke(false);





    }

    public override void ExitState(PlayerStateManager player)
    {
        player.ID.UiEvents.OnShowSyctheLine?.Invoke(false);

    }

    public override void PressButton(PlayerStateManager player, Vector2 startPos)
    {
        startPoint = startPos;



    }

    public override void ReleaseButton(PlayerStateManager player)
    {
        sentSwipeData = false;
    }

    public override void CollectAmmo(PlayerStateManager player, int type)
    {
        if (type == ammoType) player.ID.UiEvents.OnUseAmmo?.Invoke(player.ID.Ammo);
    }
    public override void SwipeButton(PlayerStateManager player, Vector2 pos)
    {
        if (sentSwipeData) return;
        float dragDistance = Vector2.Distance(startPoint, pos);

        if (dragDistance > dragThreshold)
        {

            bool isLeftSide = startPoint.x < scytheSwipeCenterPos.x;
            bool under = startPoint.y < scytheSwipeCenterPos.y;



            sentSwipeData = true;
            // Calculate the angle of the swipe in degrees
            Vector2 direction = (pos - startPoint).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            if (MathF.Abs(angle - 315) < angleSwipeMinDiff)
            {
                Debug.Log("Checking Angle, x and y diff of start is: " + xDistance(startPoint.x) + ", " + yDistance(startPoint.y));
                if (xDistance(startPoint.x) < yDistance(startPoint.y))
                {

                    player.GetScytheSwipeAttack(315);
                    return;

                }
                else
                {

                    player.GetScytheSwipeAttack(-135);
                    return;

                }


            }
            else if (MathF.Abs(angle - 45) < angleSwipeMinDiff)
            {
                if (xDistance(startPoint.x) < yDistance(startPoint.y))
                {
                    player.GetScytheSwipeAttack(45);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(-225);
                    return;
                }


            }
            else if (MathF.Abs(angle - 135) < angleSwipeMinDiff)
            {
                if (xDistance(startPoint.x) < yDistance(startPoint.y))
                {
                    player.GetScytheSwipeAttack(135);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(-300);
                    return;
                }


            }
            else if (MathF.Abs(angle - 225) < angleSwipeMinDiff)
            {
                if (xDistance(startPoint.x) < yDistance(startPoint.y))
                {
                    player.GetScytheSwipeAttack(225);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(-350);
                    return;
                }


            }

            Debug.Log("Not using angles");


            if (angle < 45 || angle > 315)
            {
                if (under)
                {
                    player.GetScytheSwipeAttack(1);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(0);
                    return;
                }
            }

            else if (angle < 225 && angle > 135)
            {
                if (under)
                {
                    player.GetScytheSwipeAttack(179);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(180);
                    return;
                }
            }

            else if (angle > 45 && angle < 135)
            {
                if (isLeftSide)
                {
                    Debug.Log("LEft side up");
                    player.GetScytheSwipeAttack(91);
                    return;
                }
                else
                {
                    Debug.Log("Right side up");

                    player.GetScytheSwipeAttack(90);
                    return;
                }
            }
            else if (angle > 225 && angle < 315)
            {
                if (isLeftSide)
                {
                    player.GetScytheSwipeAttack(269);
                    return;
                }
                else
                {
                    player.GetScytheSwipeAttack(270);
                    return;
                }
            }


        }
    }

    private float xDistance(float x)
    {
        return MathF.Abs(x - scytheSwipeCenterPos.x);
    }
    private float yDistance(float y)
    {
        return MathF.Abs(y - scytheSwipeCenterPos.y);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}