using UnityEngine;
using UnityEngine.UI;

public class ControlIndicatorUI : MonoBehaviour
{
    [Header("Groups")]
    public GameObject wasdGroup;
    public GameObject micGroup;

    [Header("WASD Keys")]
    public Image keyW;
    public Image keyA;
    public Image keyS;
    public Image keyD;

    [Header("Mic")]
    public Image micImage;
    private float micDisplayTimer = 0f;
    public float micDisplayDuration = 0.5f;

    [Header("Colors")]
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    public Color activeColor = new Color(1f, 1f, 1f, 1f);

    private ControlMode currentMode = ControlMode.Keyboard;

    void Update()
    {
        if (currentMode == ControlMode.Keyboard)
        {
            UpdateKey(keyW, KeyCode.W);
            UpdateKey(keyA, KeyCode.A);
            UpdateKey(keyS, KeyCode.S);
            UpdateKey(keyD, KeyCode.D);
        }

        if (currentMode == ControlMode.Voice && micImage != null)
        {
            if (micDisplayTimer > 0)
            {
                micDisplayTimer -= Time.deltaTime;
                micImage.color = activeColor;
            }
            else
            {
                micImage.color = inactiveColor;
            }
        }
    }


    void UpdateKey(Image keyImage, KeyCode key)
    {
        if (keyImage != null)
            keyImage.color = Input.GetKey(key) ? activeColor : inactiveColor;
    }
    public void TriggerMicActivity()
    {
        micDisplayTimer = micDisplayDuration;
    }

    public void SetControlMode(ControlMode mode)
    {
        currentMode = mode;

        if (wasdGroup != null) wasdGroup.SetActive(mode == ControlMode.Keyboard);
        if (micGroup != null) micGroup.SetActive(mode == ControlMode.Voice);

        if (micImage != null)
            micImage.color = mode == ControlMode.Voice ? activeColor : inactiveColor;
    }
}
