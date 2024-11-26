using UnityEngine;

public class MaterialPassInspector : MonoBehaviour
{
    [SerializeField] private PreloadAssetsContainer preLoadAssets; // Assign your PreloadAssetsContainer here

    void Start()
    {
        if (preLoadAssets == null || preLoadAssets.materials == null || preLoadAssets.materials.Length == 0)
        {
            Debug.LogError("No materials found in PreloadAssetsContainer.");
            return;
        }

        foreach (Material material in preLoadAssets.materials)
        {
            if (material == null || material.shader == null)
            {
                Debug.LogWarning("Material or its shader is null. Skipping...");
                continue;
            }

            Shader shader = material.shader;
            Debug.Log($"Inspecting material: {material.name}, Shader: {shader.name}");

            for (int i = 0; i < material.passCount; i++) // material.passCount gives the number of passes
            {
                string passName = material.GetPassName(i); // Get the name of the pass
                string lightMode = material.GetTag("LightMode", false, ""); // Get the LightMode tag

                Debug.Log($"Pass {i}: Name = {passName}, LightMode = {lightMode}");
            }
        }
    }
}