using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerMovementDataButton : MonoBehaviour
{
    [SerializeField] private PlayerMovementData data;
    [SerializeField] private PlayerID player;




    // Start is called before the first frame update

    // Update is called once per frame
    public void OnPress()
    {
        Debug.Log("Invoking Event");
        player.globalEvents.OnSetNewPlayerMovementData?.Invoke(data);

    }
}
