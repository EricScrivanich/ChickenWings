#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class ShaderVariantCollectionPopulator : MonoBehaviour
{
    [SerializeField] private PreloadAssetsContainer preLoadAssets;
    [SerializeField] private ShaderVariantCollection shaderVariantCollection;

    [ContextMenu("Populate Shader Variant Collection")]
    public void PopulateShaderVariantCollection()
    {
        if (preLoadAssets == null || preLoadAssets.materials == null || preLoadAssets.materials.Length == 0)
        {
            Debug.LogError("PreloadAssetsContainer or materials list is not assigned.");
            return;
        }

        if (shaderVariantCollection == null)
        {
            Debug.LogError("Shader Variant Collection asset is not assigned.");
            return;
        }

        foreach (Material material in preLoadAssets.materials)
        {
            if (material == null || material.shader == null) continue;

            Shader shader = material.shader;
            string[] keywords = material.shaderKeywords;

            Debug.Log($"Inspecting material: {material.name}, Shader: {shader.name}, Keywords: [{string.Join(", ", keywords)}]");

            foreach (PassType passType in new[] { PassType.ScriptableRenderPipeline, PassType.ShadowCaster, PassType.ForwardBase,  })
            {
                try
                {
                    ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(
                        shader,
                        passType,
                        keywords
                    );

                    if (!shaderVariantCollection.Contains(variant))
                    {
                        shaderVariantCollection.Add(variant);
                        Debug.Log($"Added variant: Shader = {shader.name}, PassType = {passType}, Keywords = [{string.Join(", ", keywords)}]");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to add variant: Shader = {shader.name}, PassType = {passType}, Keywords = [{string.Join(", ", keywords)}]. Error: {ex.Message}");
                }
            }
        }

        // Save the updated Shader Variant Collection
        EditorUtility.SetDirty(shaderVariantCollection);
        AssetDatabase.SaveAssets();
        Debug.Log("Shader Variant Collection populated and saved.");
    }
}
#endif