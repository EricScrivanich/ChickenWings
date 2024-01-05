using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScripts : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerMovement movementScript;
    private PowerUps powerUpScript;
    private eggStuff eggScript;
    // Start is called before the first frame update
    void Start()
    {
        movementScript = player.GetComponent<PlayerMovement>();
        powerUpScript = player.GetComponent<PowerUps>();
        eggScript = player.GetComponent<eggStuff>();
    }

    // Update is called once per frame
  
    public void Fireball()
    {
        powerUpScript.Fireball();
    }

    public void Jump()
    {
        movementScript.Jump();
    }

   

   

    public void DropEgg()
    {
        eggScript.DropEgg();
    }
    public void JumpLeft()
    {
        movementScript.JumpLeft();
    }

    public void JumpRight()
    {
        movementScript.JumpRight();
    }
}
