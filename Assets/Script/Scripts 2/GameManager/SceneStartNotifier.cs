using UnityEngine;
using TMPro;

public class SceneStartNotifier : MonoBehaviour
{
    public TMP_Text notificationText;
    public CanvasGroup canvasGroup;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

    void Start()
    {
        GameCompletionFlow flow = FindObjectOfType<GameCompletionFlow>();

        if (notificationText != null && canvasGroup != null && flow != null)
        {
            notificationText.text = $"Eat {flow.targetGarbageCount} Garbage";
            canvasGroup.alpha = 1;

            LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
                     .setDelay(displayTime)
                     .setIgnoreTimeScale(true);
        }
    }
}
