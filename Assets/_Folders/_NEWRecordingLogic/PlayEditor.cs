using UnityEngine;
using TMPro;

public class PlayEditor : MonoBehaviour
{
    [SerializeField] private StartingStatEditor ammoEditor;
    private int currentAmmoType = -2;

    [SerializeField] private StartingStatDisplay[] ammoDisplays;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    private void Start()
    {
        currentAmmoType = -2;
        for (int i = 0; i < ammoDisplays.Length; i++)
        {
            if (ammoDisplays[i] != null)
            {
                ammoDisplays[i].SetPlayEditor(this);
            }
        }

        // ammoDisplays[0].OnPointerDown(null);
    }

    public void OnSelectAmmo(int type, Sprite img, TextMeshProUGUI text)
    {
        if (currentAmmoType == type) return;
        currentAmmoType = type;
        ammoEditor.SetNewEgg(type, img, text);

        for (int i = 0; i < ammoDisplays.Length; i++)
        {
            if (ammoDisplays[i] != null)
            {
                ammoDisplays[i].SetNewEgg(type);
            }
        }

    }
}
