using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SharkStats : MonoBehaviour
{
    public enum SharkSize { Small, Medium, Big }
    public SharkSize currentSize = SharkSize.Small;

    private int currentScore = 0;
    private int garbageEaten = 0;

    [Header("Growth by Garbage Eaten")]
    public int mediumSizeThreshold = 3;
    public int bigSizeThreshold = 6;

    [Header("Growth Settings")]
    public float growthScaleMultiplier = 1.5f;

    [Header("Point Popup")]
    public GameObject pointsPopupPrefab;
    public Canvas pointsPopupCanvas;

    [Header("UI Progress")]
    public Slider levelProgressBar;
    public TMP_Text levelLabel;

    public GameObject tooBigPopupPrefab;
    public RectTransform popupParent;

    [Header("Combo System")]
    public float comboDuration = 2f;
    public int maxComboMultiplier = 5;
    private float comboTimer = 0f;
    private int comboMultiplier = 1;

    private Coroutine progressAnimCoroutine;
    private GameCompletionFlow gameCompletion;

    private void Start()
    {
        gameCompletion = FindObjectOfType<GameCompletionFlow>();
        UpdateScoreUIWithCombo(0);
        UpdateProgressUI();
    }

    private void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboMultiplier = 1;

                var scoreManager = FindObjectOfType<ScoreManager>();
                if (scoreManager != null && scoreManager.comboText != null)
                {
                    LeanTween.alphaText(scoreManager.comboText.rectTransform, 0f, 0.5f).setOnComplete(() => {
                        scoreManager.comboText.gameObject.SetActive(false);
                    });
                }
            }
        }
    }

    public void AddScore(int points)
    {
        garbageEaten++;

        if (comboTimer > 0f)
        {
            comboMultiplier = Mathf.Min(comboMultiplier + 1, maxComboMultiplier);
        }
        comboTimer = comboDuration;


        int finalPoints = points * comboMultiplier;
        currentScore += finalPoints;

        UpdateScoreUIWithCombo(finalPoints);
        UpdateProgressUI();

        if (currentSize == SharkSize.Small && garbageEaten >= mediumSizeThreshold)
            GrowTo(SharkSize.Medium);
        else if (currentSize == SharkSize.Medium && garbageEaten >= bigSizeThreshold)
            GrowTo(SharkSize.Big);

        FindObjectOfType<GameCompletionFlow>()?.RegisterGarbageEaten(finalPoints);
        ShowPointsPopup(finalPoints);
    }

    private void UpdateScoreUIWithCombo(int gainedPoints)
    {
        var scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.UpdateScore(currentScore);

            if (comboMultiplier > 1 && gainedPoints > 0)
            {
                scoreManager.ShowComboText(" (x" + comboMultiplier + ")");
            }
        }
    }

    public void ResetCombo()
    {
        comboMultiplier = 1;
        comboTimer = 0f;

        var scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null && scoreManager.comboText != null)
        {
            LeanTween.cancel(scoreManager.comboText.gameObject);
            LeanTween.alphaText(scoreManager.comboText.rectTransform, 0f, 0.5f).setOnComplete(() => {
                scoreManager.comboText.gameObject.SetActive(false);
            });
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void GrowTo(SharkSize newSize)
    {
        currentSize = newSize;
        transform.localScale *= growthScaleMultiplier;
        Debug.Log("Shark grew to: " + newSize);
    }

    private void UpdateProgressUI()
    {
        if (levelProgressBar == null) return;

        int targetCount = gameCompletion != null ? gameCompletion.targetGarbageCount : 9;
        float progress = Mathf.Clamp01((float)garbageEaten / targetCount);

        if (progressAnimCoroutine != null)
            StopCoroutine(progressAnimCoroutine);

        progressAnimCoroutine = StartCoroutine(AnimateProgressBar(progress));

        if (levelLabel != null)
            levelLabel.text = "SIZE: " + currentSize.ToString();
    }

    private IEnumerator AnimateProgressBar(float targetValue)
    {
        float duration = 0.3f;
        float time = 0f;
        float startValue = levelProgressBar.value;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            levelProgressBar.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }

        levelProgressBar.value = targetValue;
    }

    public bool CanEat(GarbageType.GarbageSize garbageSize)
    {
        switch (currentSize)
        {
            case SharkSize.Small:
                return garbageSize == GarbageType.GarbageSize.Small;
            case SharkSize.Medium:
                return garbageSize == GarbageType.GarbageSize.Small || garbageSize == GarbageType.GarbageSize.Medium;
            case SharkSize.Big:
                return true;
        }
        return false;
    }

    public void ShowTooBigPopup()
    {
        if (tooBigPopupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(tooBigPopupPrefab, popupParent);
            RectTransform rt = popup.GetComponent<RectTransform>();

            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            rt.position = screenPos;

            CanvasGroup cg = popup.GetComponent<CanvasGroup>();
            cg.alpha = 1;

            LeanTween.moveY(rt, rt.anchoredPosition.y + 100f, 0.5f)
                .setEaseOutCubic()
                .setIgnoreTimeScale(true);

            LeanTween.alphaCanvas(cg, 0f, 0.5f)
                .setDelay(0.3f)
                .setIgnoreTimeScale(true);

            Destroy(popup, 1f);
        }
    }

    private void ShowPointsPopup(int points)
    {
        if (pointsPopupPrefab != null && pointsPopupCanvas != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject popup = Instantiate(pointsPopupPrefab, screenPos, Quaternion.identity, pointsPopupCanvas.transform);

            PointsPopup popupScript = popup.GetComponent<PointsPopup>();
            if (popupScript != null)
            {
                popupScript.SetPoints(points);
            }
        }
    }
}
