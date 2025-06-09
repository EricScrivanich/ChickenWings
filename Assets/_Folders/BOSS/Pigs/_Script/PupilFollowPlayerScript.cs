using UnityEngine;

public class PupilFollowPlayerScript : MonoBehaviour
{

    [SerializeField] private Vector2 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
   

    // private void Follow(Vector2 playerPosition)
    // {
    //     Vector2 direction = player.position - pupil.position; // Calculate the direction to the player
    //                                                           // Ensure it's 2D
    //     direction.Normalize(); // Normalize the direction

    //     if (flipped)
    //     {
    //         direction *= flippedDirection;
    //     }

    //     // Move the pupil within the eye's radius
    //     pupil.localPosition = direction * eyeRadius;
    // }
}
