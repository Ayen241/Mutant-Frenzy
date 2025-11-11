using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [Header("Skybox Rotation Settings")]
    [Tooltip("Speed of skybox rotation in degrees per second")]
    public float skyRotationSpeed = 1f;

    private float currentRotation = 0f;

    void Update()
    {
        if (RenderSettings.skybox.HasProperty("_Rotation"))
        {
            currentRotation += skyRotationSpeed * Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Rotation", currentRotation % 360);
        }
        else
        {
            Debug.LogWarning("Skybox material doesn't support _Rotation.");
        }
    }
}
