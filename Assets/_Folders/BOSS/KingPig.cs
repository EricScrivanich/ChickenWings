using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPig : MonoBehaviour
{
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float spinningMoveSpeed;
    [SerializeField] private Transform target;
    [SerializeField] private float swingCooldown;
    [SerializeField] private float spinDuration;
    private Animator anim;

    private bool spinning = false;
    private bool moveNormal = true;
    private int currentSwings = 0;
    private float swingTime = 0;
    private float spinTime = 0;
    [SerializeField] private int amountOfSwings;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (moveNormal)
        {
            swingTime += Time.deltaTime;
            var step = normalMoveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target.position) < 2.6f && swingTime > swingCooldown)
            {
                // Swap the position of the cylinder.
                anim.SetTrigger("SwingNormal");
                currentSwings++;
                swingTime = 0;

                if (currentSwings >= amountOfSwings)
                {
                    anim.SetTrigger("Spin");
                    moveNormal = false;
                    Invoke("SetSpin", 2f);
                }
                else
                {
                    anim.SetTrigger("SwingRevert");
                }

            }
        }


        else if (spinning)
        {
            spinTime += Time.deltaTime;
            var step = spinningMoveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            if (spinTime > spinDuration)
            {
                spinning = false;
                spinTime = 0;
                anim.SetTrigger("SpinRevert");
                Invoke("SetSwings", 2f);
            }

        }

    }





    private void SetSpin()
    {
        spinning = true;
    }

    private void SetSwings()
    {
        moveNormal = true;
    }
}
