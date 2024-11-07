using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerMovementDataButton : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private PlayerID player;

    // private static Action<int> OnSelectNewStats;

    private Image img;


    private void Awake()
    {
        img = GetComponent<Image>();

        if (id == 0)
            img.color = Color.green;

    }

    // Start is called before the first frame update

    // Update is called once per frame
    public void OnPress()
    {
        Debug.Log("Invoking Event");
        player.globalEvents.OnSetNewPlayerMovementData?.Invoke(data);

        img.color = Color.green;

    }

    private void ChangeColor(PlayerMovementData d)
    {
        if (d != data) img.color = Color.white;
    }

    private void OnEnable()
    {

        player.globalEvents.OnSetNewPlayerMovementData += ChangeColor;

    }
    private void OnDisable()
    {
        player.globalEvents.OnSetNewPlayerMovementData -= ChangeColor;

    }

}
