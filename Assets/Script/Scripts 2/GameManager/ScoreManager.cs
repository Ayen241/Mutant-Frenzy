using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text comboText;

    void Start()
    {
        // Hide combo text at start
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
            RectTransform rt = comboText.rectTransform;
            rt.localScale = Vector3.one;
            LeanTween.scale(rt, Vector3.one * 1.075f, 0.6f).setEaseInOutSine().setLoopPingPong();
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score " + score;

            // Softer bouncy animation on score text
            RectTransform rt = scoreText.rectTransform;
            rt.localScale = Vector3.one;
            LeanTween.cancel(scoreText.gameObject);
            LeanTween.scale(rt, Vector3.one * 1.08f, 0.2f).setEaseOutBack().setOnComplete(() => {
                LeanTween.scale(rt, Vector3.one, 0.1f);
            });
        }
    }

    public void ShowComboText(string text)
    {
        if (comboText != null)
        {
            comboText.text = text;
            comboText.gameObject.SetActive(true);
            comboText.alpha = 1f;
        }
    }

    public void HideComboText()
    {
        if (comboText != null)
        {
            LeanTween.cancel(comboText.gameObject);
            LeanTween.alphaText(comboText.rectTransform, 0f, 0.5f).setOnComplete(() => {
                comboText.gameObject.SetActive(false);
            });
        }
    }
}
