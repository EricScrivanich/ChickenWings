using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private float minGravity = 500f;
    [SerializeField] private float maxGravity = 2000f;
    [SerializeField] private float minRotationSpeed = 10f;
    [SerializeField] private float maxRotationSpeed = 30f;

    [SerializeField] private Button[] buttons;

    public bool buttonPressed;

    void Awake()
    {
      buttonPressed = false;
    }

    private void Start()
    {
       
    }
    public void LoadSceneWithIndex(int sceneIndex)
{
    StartCoroutine(LoadSceneCoroutine(sceneIndex));
}
    
    
     private IEnumerator LoadSceneCoroutine(int sceneIndex)
{
    buttonPressed = true;
    // Disable button interaction
    foreach (var button in buttons)
    {
        button.interactable = false;
    }

        float timer = 0f;
        Vector3[] originalPositions = new Vector3[buttons.Length];
        float[] gravityValues = new float[buttons.Length];
        float[] rotationSpeeds = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            originalPositions[i] = buttons[i].transform.position;
            gravityValues[i] = Random.Range(minGravity, maxGravity);
            rotationSpeeds[i] = Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.value > 0.5f ? 1f : -1f);
        }

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            // Apply gravity and rotation to buttons
            for (int i = 0; i < buttons.Length; i++)
            {
                Vector3 position = originalPositions[i];
                position.y -= gravityValues[i] * Mathf.Pow(timer / fallDuration, 2f);
                buttons[i].transform.position = position;

                buttons[i].transform.Rotate(Vector3.forward, rotationSpeeds[i] * Time.deltaTime);
            }

         
            yield return null;
        }

        // Load the next scene
      SceneManager.LoadScene(sceneIndex);
    }
}
