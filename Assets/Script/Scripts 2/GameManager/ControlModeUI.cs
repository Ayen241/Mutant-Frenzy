using TMPro;
using UnityEngine;

public class ControlModeUI : MonoBehaviour
{
    public TMP_Text controlModeText;
    public GameObject controlNotification;
    public float notificationDuration = 2f;
    public GameObject keyboardInfoPanel;
    public GameObject voiceInfoPanel;


    public void UpdateControlModeText(ControlMode mode)
    {
        if (controlModeText != null)
        {
            controlModeText.text = mode == ControlMode.Keyboard ? "KEYBOARD" : "VOICE";
        }
    }

    public void ShowControlNotification(ControlMode mode)
    {
        if (controlNotification == null) return;

        LeanTween.cancel(controlNotification);

        RectTransform rect = controlNotification.GetComponent<RectTransform>();
        CanvasGroup cg = controlNotification.GetComponent<CanvasGroup>();
        TMP_Text text = controlNotification.GetComponent<TMP_Text>();

        rect.anchoredPosition = new Vector2(0, 0);
        cg.alpha = 1f;
        text.text = mode == ControlMode.Voice ? "Use Your Voice!!" : "Use Keyboard";

        controlNotification.SetActive(true);

        LeanTween.moveY(rect, 100f, notificationDuration)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true);

        LeanTween.alphaCanvas(cg, 0f, notificationDuration)
            .setEaseInOutQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => controlNotification.SetActive(false));
    }
    public void ShowActiveControlInfo()
    {
        if (keyboardInfoPanel != null)
            keyboardInfoPanel.SetActive(false);
        if (voiceInfoPanel != null)
            voiceInfoPanel.SetActive(false);

        if (PlayerPrefs.HasKey("controlMode"))
        {
            ControlMode mode = (ControlMode)PlayerPrefs.GetInt("controlMode");
            GameObject targetPanel = null;

            if (mode == ControlMode.Keyboard)
                targetPanel = keyboardInfoPanel;
            else if (mode == ControlMode.Voice)
                targetPanel = voiceInfoPanel;

            if (targetPanel != null)
            {
                targetPanel.SetActive(true);

                RectTransform rt = targetPanel.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localScale = Vector3.zero; // start small
                    LeanTween.scale(rt, Vector3.one, 0.4f)
                        .setEaseOutBack()
                        .setIgnoreTimeScale(true); // important!
                }
            }
        }
    }
    public void CloseInfoPanels()
    {
        void AnimateClose(GameObject panel)
        {
            if (panel == null || !panel.activeSelf) return;

            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null)
            {
                LeanTween.scale(rt, Vector3.zero, 0.3f)
                    .setEaseInBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => panel.SetActive(false));
            }
            else
            {
                panel.SetActive(false); // fallback
            }
        }

        AnimateClose(keyboardInfoPanel);
        AnimateClose(voiceInfoPanel);
    }


}
