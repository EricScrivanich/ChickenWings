using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform targetTransform;
    private SpriteRenderer sprite;
    private float YPos = 3.65f;
    private float leftXPos = -9.65f;
    private float rightXPos = 9.65f;
    private int type;

    private Vector3 leftRotation = new Vector3(0, 0, 90);
    private Vector3 rightRotation = new Vector3(0, 0, -90);

    [SerializeField] private Color originalColor;
    [SerializeField] private Color frozenColor;
    [SerializeField] private Color chicCageColor;

    private bool isLeft;
    private bool isTop;
    // Start is called before the first frame update

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();


    }

    public void InitializeArrow(Transform target, int t)
    {
        targetTransform = target;
        type = t;
        if (type == 1)
        {
            sprite.color = chicCageColor;
            transform.localScale = new Vector3(.9f, .9f, .9f);
        }




    }
    // Update is called once per frame
    void LateUpdate()
    {

        if (isTop)
        {
            transform.position = new Vector2(targetTransform.position.x, YPos);
            if (targetTransform.position.y > BoundariesManager.TopPlayerBoundary && type == 0)
            {
                sprite.color = frozenColor;

            }

        }
        else if (isLeft) transform.position = new Vector2(leftXPos, targetTransform.position.y);
        else transform.position = new Vector2(rightXPos, targetTransform.position.y);

    }

    public void SetActive(int pos)
    {
        if (pos == 0)
        {
            isTop = true;
            transform.eulerAngles = Vector3.zero;

        }
        else if (pos == 1)
        {
            isLeft = true;
            isTop = false;
            transform.eulerAngles = leftRotation;

        }
        else if (pos == 2)
        {
            isLeft = false;
            isTop = false;
            transform.eulerAngles = rightRotation;
        }
        if (type == 0)
            sprite.color = originalColor;


        gameObject.SetActive(true);

    }
}
