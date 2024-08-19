using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCheckPoint : MonoBehaviour
{
    [SerializeField] private int CheckPointID;
    [SerializeField] private Transform setPlayerPosition;
    [SerializeField] private GameObject ShowSignSection;
    // Start is called before the first frame update
    void Awake()
    {

        
    }

    private void IniitializeCheckPoint(int id)
    {
        if (id == CheckPointID)
        {
           
        }
    }

    private void HandlePlayer()
    {
        var playerScript = GameObject.Find("Player").GetComponent<PlayerStateManager>();

        playerScript.SwitchState(playerScript.IdleState);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
