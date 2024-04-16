using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] private bool StayActive;
    // Start is called before the first frame update
    public GameObject virtualCamera;


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            virtualCamera.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            virtualCamera.SetActive(false);
            gameObject.SetActive(StayActive);

        }
        
    }
}
