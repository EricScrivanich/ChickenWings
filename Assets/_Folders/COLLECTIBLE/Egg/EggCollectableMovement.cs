using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectableMovement : MonoBehaviour, ICollectible
{

    public PlayerID ID;
    private float speed;
    private int ammoAmount;

    [SerializeField] private float eggTime;
    private float minY = -2.5f;
    private float maxY = 4.5f;
    private float yPos;

    private bool isPostiveXSpeed;


    private float minAmmoSpeed = 6f;
    private float maxAmmoSpeed = 8.5f;

    private float minEggSinFrequency = .5f;
    private float maxEggSinFrequency = .8f;
    private float eggSinFrequency;


    private float minEggSinAmplitude = .5f;
    private float maxEggSinAmplitude = .9f;
    private float eggSinAmplitude;


    public bool isThreeAmmo;
    [SerializeField] private SpriteRenderer threeImage;


    public void EnableAmmo(bool isThree, float speedVar)
    {
        if (this.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        isThreeAmmo = isThree;

        if (!isThreeAmmo)
        {
            eggSinFrequency = Random.Range(.1f, .5f);
            eggSinAmplitude = Random.Range(.3f, .7f);
            ammoAmount = 1;
        }
        else if (isThreeAmmo)
        {
            eggSinFrequency = Random.Range(.4f, .75f);
            eggSinAmplitude = Random.Range(.5f, .9f);
            ammoAmount = 3;
        }

        if (speedVar == 0)
        {
            speed = Random.Range(5.5f, 8.5f);
        }
        else speed = speedVar;

        yPos = transform.position.y;
        isPostiveXSpeed = speed > 0;
        threeImage.enabled = isThreeAmmo;

        this.gameObject.SetActive(true);





    }
    private void OnEnable()
    {

        if (!isThreeAmmo)
        {
            eggSinFrequency = Random.Range(.1f, .5f);
            eggSinAmplitude = Random.Range(.3f, .7f);
            ammoAmount = 1;
        }
        else if (isThreeAmmo)
        {
            eggSinFrequency = Random.Range(.4f, .75f);
            eggSinAmplitude = Random.Range(.5f, .9f);
            ammoAmount = 3;
        }

        if (speed == 0)
        {
            speed = Random.Range(5.5f, 8.5f);
        }
        yPos = transform.position.y;
        isPostiveXSpeed = speed > 0;
        threeImage.enabled = isThreeAmmo;


    }


    void Update()
    {

        transform.Translate(Vector3.left * speed * Time.deltaTime);
        float x = transform.position.x;
        float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

        if (isPostiveXSpeed)
        {
            if (transform.position.x < -13) gameObject.SetActive(false);
        }
        else
        {
            if (transform.position.x > 13) gameObject.SetActive(false);
        }




    }
    void Movement()
    {

        transform.Translate(Vector3.left * speed * Time.deltaTime);
        float x = transform.position.x;
        float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

        if (isPostiveXSpeed)
        {
            if (transform.position.x < -13) gameObject.SetActive(false);
        }
        else
        {
            if (transform.position.x > 13) gameObject.SetActive(false);
        }


    }


    public void Collected()
    {
        ID.Ammo += ammoAmount;
        ID.globalEvents.OnUpdateAmmo?.Invoke();
        gameObject.SetActive(false);

    }
}
