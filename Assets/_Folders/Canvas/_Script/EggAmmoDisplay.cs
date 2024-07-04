using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EggAmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    private Image ButtonImage;
    [SerializeField] private Image EggImage;
    [SerializeField] private Color outOfAmmoTextColor;
    [SerializeField] private Color outOfAmmoButtonColor;
    [SerializeField] private Color outOfAmmoEggColor;
    [SerializeField] private Color ammoTextColor;
    [SerializeField] private Color eggColor;
    [SerializeField] private bool usingEggs;

    private bool over10;
    private bool over100;
    private bool outOfAmmo;
    public PlayerID player;
    private int previousAmmo;


    // Start is called before the first frame update
    void Start()
    {
        ButtonImage = GetComponent<Image>();
        ammoText.text = player.Ammo.ToString();
        outOfAmmo = player.Ammo < 1;

        if (outOfAmmo)
        {
            ButtonImage.color = outOfAmmoButtonColor;
            ammoText.color = outOfAmmoTextColor;
            EggImage.color = outOfAmmoEggColor;

        }
        else
        {
            ButtonImage.color = Color.white;
            ammoText.color = ammoTextColor;
            EggImage.color = eggColor;


        }

        CheckTextSize(player.Ammo);


    }
    private void OnEnable()
    {
        player.globalEvents.OnUpdateAmmo += UpdateAmmo;
    }
    private void OnDisable()
    {
        player.globalEvents.OnUpdateAmmo -= UpdateAmmo;
    }

    private void CheckTextSize(int number)
    {
        if (number > 99 && !over100)
        {
            ammoText.fontSize = 90;
            over100 = true;


        }
        else if (number < 100 && over100)
        {
            over100 = false;
            ammoText.fontSize = 118;
            over10 = true;
        }
        else if (number > 9 && !over10 && !over100)
        {
            ammoText.fontSize = 118;
            over10 = true;
        }
        else if (number < 10 && over10)
        {
            ammoText.fontSize = 155;
            over10 = false;

        }
    }
    void UpdateAmmo()
    {
        int currentAmmo = player.Ammo;
        if (currentAmmo == 0)
        {
            if (!outOfAmmo)
            {
                ButtonImage.color = outOfAmmoButtonColor;
                ammoText.color = outOfAmmoTextColor;
                EggImage.color = outOfAmmoEggColor;

                outOfAmmo = true;
            }

        }
        else if (outOfAmmo)
        {
            ButtonImage.color = Color.white;
            ammoText.color = ammoTextColor;
            EggImage.color = eggColor;
            outOfAmmo = false;
        }
        CheckTextSize(currentAmmo);
        ammoText.text = currentAmmo.ToString();

    }

    // Update is called once per frame
}
