using UnityEngine;

public class Level1LightingOverride : MonoBehaviour
{
    public Light levelLight;
    public Material levelSkybox;

    void Start()
    {
        // Optional: destroy leftover directional lights
        foreach (var light in GameObject.FindObjectsOfType<Light>())
        {
            if (light != levelLight && light.type == LightType.Directional)
                Destroy(light.gameObject);
        }

        // Force correct skybox and sun
        if (levelSkybox != null) RenderSettings.skybox = levelSkybox;
        if (levelLight != null) RenderSettings.sun = levelLight;

        DynamicGI.UpdateEnvironment();
    }
}
