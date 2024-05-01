using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBOundary : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        arrow.SetActive(true);

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        arrow.SetActive(false);

    }
}
