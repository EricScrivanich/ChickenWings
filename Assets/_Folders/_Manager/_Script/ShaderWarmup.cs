using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShaderWarmup : MonoBehaviour
{
    [SerializeField] private List<Material> materialsToWarm;
    [SerializeField] private string nextSceneName = "MainMenu";

    IEnumerator Start()
    {
        // Wait 1 frame so all preloaded shaders are ready
        yield return null;

        // Create a tiny hidden camera
        Camera hiddenCam = new GameObject("HiddenWarmupCam").AddComponent<Camera>();
        hiddenCam.enabled = false; // we manually force a render

        // Create a small quad to draw all materials
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = new Vector3(0, 0, 2);
        quad.SetActive(false);

        // Force GPU to render EACH material once
        foreach (var mat in materialsToWarm)
        {
            if (mat == null) continue;

            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;

            // Enable → Render → Disable
            quad.SetActive(true);
            hiddenCam.Render();
            quad.SetActive(false);

            yield return null; // small yield to avoid frame blocking
        }

        Destroy(quad);
        Destroy(hiddenCam.gameObject);

        // Now load the main menu
        SceneManager.LoadScene(nextSceneName);
    }
}