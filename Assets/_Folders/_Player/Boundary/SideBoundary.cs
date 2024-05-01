using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoundary : MonoBehaviour
{
    public PlayerID player;
    // Start is called before the first frame update
 private void OnTriggerEnter2D(Collider2D other) {
        player.globalEvents.OnOffScreen?.Invoke();
    }
}
