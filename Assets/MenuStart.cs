using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStart : MonoBehaviour
{
    public PlayerID player;
 
 private void Awake() {
        player.constantPlayerForceBool = false;
    }
 
    // Start is called before the first frame update
}
