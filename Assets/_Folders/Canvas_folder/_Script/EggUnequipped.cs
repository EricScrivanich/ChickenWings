using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class EggUnequipped : MonoBehaviour
{
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private PlayerID player;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Vector2 minMaxMoveY;
    [SerializeField] private Vector2 mainPos1;
    [SerializeField] private Vector2 underPos1;
    [SerializeField] private Vector2 hiddenPos1;
    [SerializeField] private Vector2 hiddenPos2;

    [SerializeField] private int ammoType;

    private bool isShown;


    private RectTransform rect;




    private int currentAmmo = 4;
    // Start is called before the first frame update
    void Start()
    {

        switch (ammoType)
        {
            case (0):

                UpdateAmmo(player.Ammo);
                player.globalEvents.OnUpdateAmmo += UpdateAmmo;



                break;
            case (1):
                UpdateAmmo(player.ShotgunAmmo);

                player.globalEvents.OnUpdateShotgunAmmo += UpdateAmmo;


                break;
        }


    }
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        // EggAmmoDisplay.SwitchAmmoEvent += HandleSwitchAmmo;
        // EggUnequippedParent.OnHideEggButton += HideEggButton;

    }


    private void UnEquip(int type)
    {

    }

    private void Equip(int type)
    {

    }

    private void HandleSwitchAmmo(int enteringEggType, int exitingEggType)
    {
        if (ammoType == enteringEggType)
        {
            isShown = true;
            rect.DOLocalMove(mainPos1, .25f);

        }
        else if (ammoType == exitingEggType)
        {
            isShown = false;

            rect.DOLocalMove(underPos1, .25f);

        }

    }

    private void HideEggButton(bool hide)
    {
        if (isShown) rect.DOLocalMove(hiddenPos1, .25f);
        else rect.DOLocalMove(hiddenPos2, .25f);

    }
    // Update is called once per frame

    private void OnDisable()
    {

        // EggUnequippedParent.OnHideEggButton -= HideEggButton;
        switch (ammoType)
        {
            case (0):

                player.globalEvents.OnUpdateAmmo -= UpdateAmmo;



                break;
            case (1):
                player.globalEvents.OnUpdateShotgunAmmo -= UpdateAmmo;


                break;
        }
    }
    void UpdateAmmo(int amount)
    {
        Debug.Log("Ammo update");

        text.text = amount.ToString();

        if (amount == 0 && currentAmmo != 0)
        {
            text.color = colorSO.disabledButtonColorFull;

            if (ammoType == 0)
                EggAmmoDisplay.AmmoOnZero?.Invoke(true, ammoType);

        }
        else if (amount > 0 && currentAmmo == 0)
        {
            text.color = Color.white;
            EggAmmoDisplay.AmmoOnZero?.Invoke(false, ammoType);

        }


        currentAmmo = amount;






    }
}
