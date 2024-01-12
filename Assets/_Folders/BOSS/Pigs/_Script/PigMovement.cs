// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// public class PigMovement : MonoBehaviour
// {
//     [ExposedScriptableObject]
//     public PigID ID;
//     private float elapsedTime = 0.0f;
//     private Rigidbody2D rb;
    
//     // Start is called before the first frame update
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         // StartCoroutine(Jump());

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (ID.reset)
//         {
//             // StopCoroutine(Jump());
//             rb.velocity = new Vector2(0, 0);
//             transform.position = new Vector2(0, 0);
            
//             // StartCoroutine(Jump());
//             ID.reset = false;

//         }

//         // elapsedTime += Time.deltaTime;
//         // float verticalPosition = Mathf.Sin(elapsedTime * ID.frequency) * ID.amplitude;

//         // // Use Lerp for smoother transition at the top
//         // float lerpFactor = Mathf.Abs(Mathf.Sin(elapsedTime * ID.frequency)); // This factor slows down near the top
//         // transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, verticalPosition, ID.lerpFactor));

//         elapsedTime += Time.deltaTime;

//         // Reset elapsed time after completing each cycle
//         if (elapsedTime > ID.cycleDuration)
//         {
//             elapsedTime = 0.0f;
//         }

//         // Calculate the proportion of the cycle completed
//         float cycleProgress = elapsedTime / ID.cycleDuration;

//         // Parabolic trajectory equation for vertical position
//         float verticalPosition = 4 * ID.peakHeight * cycleProgress * (1 - cycleProgress);

//         // Set the new position
//         transform.position = new Vector2(transform.position.x, verticalPosition);



//     }

        

    

//     IEnumerator Jump()
//     {
//         while(true)
//         {
//             // rb.velocity = new Vector2(0, ID.jumpForce);
//             rb.AddForce(new Vector2(0, ID.jumpForce), ForceMode2D.Impulse);
//             yield return new WaitForSeconds(ID.jumpTime);

//         }
        

       


//     }


// }
