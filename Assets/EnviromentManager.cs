using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{
    public CameraID cam;
    private BackgroundController background;
  

private void Start() {
        background = GetComponentInChildren<BackgroundController>();
    }
    private void MoveWithMountains(bool setActive, float speed)
    {
        if(!setActive)
        {
            background.MoveWithMountains(!setActive);

        }

    }

    private IEnumerator MoveWithMountainsCourintine(float speed)
    { float time = 0;
        while (time < 8)
        {
            time += Time.deltaTime;
            background.gameObject.transform.Translate(Vector2.left * speed * Time.deltaTime);
            yield return null;

        }
        background.gameObject.SetActive(false);


    }


    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
}
