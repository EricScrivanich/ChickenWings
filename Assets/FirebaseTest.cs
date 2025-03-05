// using Firebase;
// using UnityEngine;

// public class FirebaseTest : MonoBehaviour
// {
//     void Start()
//     {
//         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//         {
//             Firebase.DependencyStatus status = task.Result;
//             if (status == Firebase.DependencyStatus.Available)
//             {
//                 Debug.Log("Firebase is ready!");
//             }
//             else
//             {
//                 Debug.LogError("Could not resolve Firebase dependencies: " + status);
//             }
//         });
//     }
// }