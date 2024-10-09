using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColliderEggCoverTimer : MonoBehaviour
{
    private float lifeTimer;
    private bool enabledv = false;
    [SerializeField] private float lifetimeDuration;
    [SerializeField] private float moveDownSpeed;

    [SerializeField] private Vector2 moveDirectionSpeed;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Debug.LogError("THing eneneen: " + enabledv);
        lifeTimer = 0;
        enabledv = true;

    }
    private void OnDisable()
    {
        enabledv = false;
    }
    private void FixedUpdate()
    {

        lifeTimer += Time.fixedDeltaTime;

        if (lifeTimer >= lifetimeDuration)
        {
            gameObject.SetActive(false);
        }

        transform.Translate(moveDirectionSpeed * Time.fixedDeltaTime);

    }
}
