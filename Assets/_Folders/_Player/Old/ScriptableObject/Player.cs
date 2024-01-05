using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public PlayerID ID;
   public bool DisableButtons = false;
   public bool isFlipping = false;
   public int FlipRotVar;
   public bool justFlipped = false;
   public bool isDashing = false;
   public bool isDropping = false;

   private void Awake() {
      
   }
}
