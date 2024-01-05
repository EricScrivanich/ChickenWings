using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitle : MonoBehaviour
{
    [SerializeField] private RectTransform logoRectTransform;
    [SerializeField] private RectTransform titleRectTransform;
    [SerializeField] private float buoyancyHeight = 20f;
    [SerializeField] private float buoyancySpeed = 1f;


    [SerializeField] private float startSpeed;
    [SerializeField] private float moveUpSpeed;
    
  
     private float speed;
    

    private MainMenuScript mmScript;

    private Vector3 initialLogoPosition;

    private void Start()
    {
        mmScript = GameObject.Find("MenuButtons").GetComponent<MainMenuScript>();
        speed = startSpeed;
        initialLogoPosition = logoRectTransform.localPosition;
        StartCoroutine(BuoyantCoroutine());
    }

    private IEnumerator BuoyantCoroutine()
    {
        while (!mmScript.buttonPressed)
        {
            float newY = initialLogoPosition.y + Mathf.Sin(Time.time * buoyancySpeed) * buoyancyHeight;
            logoRectTransform.localPosition = new Vector3(initialLogoPosition.x, newY, initialLogoPosition.z);
            yield return null;
        }
    }

    private void MoveUp()
    {
        if (mmScript.buttonPressed)
        {
            speed += moveUpSpeed * Time.deltaTime;

            
            StopCoroutine(BuoyantCoroutine());
            
            logoRectTransform.Translate(Vector3.up * speed * Time.deltaTime);
            titleRectTransform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    void Update()
    {
        MoveUp();
    }
}