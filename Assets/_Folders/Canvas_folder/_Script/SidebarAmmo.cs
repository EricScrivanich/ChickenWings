using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class SidebarAmmo : MonoBehaviour
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




    }


    public void Initialize(Sprite img, int type)
    {
        GetComponent<Image>().sprite = img;
        ammoType = type;

        switch (ammoType)
        {
            case (0):

                UpdateAmmo(player.Ammo);
                player.globalEvents.OnUpdateAmmo += UpdateAmmo;



                return;
            case (1):
                UpdateAmmo(player.ShotgunAmmo);

                player.globalEvents.OnUpdateShotgunAmmo += UpdateAmmo;


                return;
        }

        UpdateAmmo(1);


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


        text.text = amount.ToString();

        if (amount == 0 && currentAmmo != 0)
        {
            text.color = colorSO.disabledButtonColorFull;

            // if (ammoType == 0)
            //     EggAmmoDisplay.AmmoOnZero?.Invoke(true, ammoType);

        }
        else if (amount > 0 && currentAmmo == 0)
        {
            text.color = Color.white;
            // EggAmmoDisplay.AmmoOnZero?.Invoke(false, ammoType);

        }


        currentAmmo = amount;






    }

    public void SendText()
    {
        Debug.Log("sent text");
        GameObject.Find("OutlineTitle").GetComponent<CustomButtonColorManager>().texts.Add(text);
    }
}
