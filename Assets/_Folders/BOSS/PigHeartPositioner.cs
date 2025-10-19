using UnityEngine;

public class PigHeartPositioner : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    public void FlipScale(bool flipped)
    {
        if (flipped)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.zero;

    }
}
