using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSetupObject : MonoBehaviour
{
    public float speed;
    public float xCordinateTrigger;

    public CollectablePoolManager manager;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);


        if (speed > 0 && transform.position.x < xCordinateTrigger)
        {
            manager.NextTrigger();
            Debug.Log("Going: " + manager.name);
            gameObject.SetActive(false);

        }
        else if (speed < 0 && transform.position.x > xCordinateTrigger)
        {
            manager.NextTrigger();
            gameObject.SetActive(false);

        }

    }
}
