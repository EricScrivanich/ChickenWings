
using UnityEngine;

public class PlaceholderPlane : MonoBehaviour
{
    public PlaneData planeType;
    public float speed;
    public int getsTriggeredInt;
    public int doesTriggerInt;
    public float xCordinateTrigger;
    private CircleCollider2D col;
    private int frameCount;

    // Start is called before the first frame update
    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;

    }

    private void FixedUpdate()
    {
        speed += Time.deltaTime;

        if (speed > xCordinateTrigger)
        {
            col.enabled = true;
            speed = 0;
            return;


        }
        else if (col.enabled == true)
        {
            frameCount++;

            if (frameCount < 4)
            {
                col.enabled = false;
            }
            return;
        }




    }


}
