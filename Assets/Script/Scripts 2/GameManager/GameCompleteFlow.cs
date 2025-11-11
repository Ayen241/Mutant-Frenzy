using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameCompletionFlow : MonoBehaviour
{
    [System.Serializable]
    public class FactSlideGroup
    {
        public Sprite[] slides;
    }

    [Header("Settings")]
    public int targetGarbageCount = 10;

    [Header("Panels")]
    public GameObject scorePanel;
    public GameObject factPanel;
    public GameObject nextLevelPanel;
    public GameObject imageCarouselPanel;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text garbageText;
    public TMP_Text factText;
    public TMP_Text nextLevelScoreText;
    public TMP_Text nextLevelGarbageText;
    public TMP_Text newRecordText;
    public TMP_Text nextLevelNewRecordText;
    public TMP_Text highScoreText;
    public TMP_Text nextLevelHighScoreText;
    public Image slideDisplay;
    public CanvasGroup endGamePanelGroup;

    [Header("Facts")]
    [TextArea] public string[] waterPollutionFacts;
    public FactSlideGroup[] factSlides;

    [Header("Star Thresholds")]
    public int oneStarScore = 50;
    public int twoStarScore = 100;
    public int threeStarScore = 150;

    [Header("Star Icons")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject nextStar1;
    public GameObject nextStar2;
    public GameObject nextStar3;

    [Header("Scene Transition")]
    public GameObject endingSceneTransition;

    [Header("Audio")]
    public AudioClip gameCompleteSound;
    public AudioSource audioSource;

    private int garbageEaten = 0;
    private int totalScore = 0;
    private int currentSlideIndex = 0;
    private Sprite[] currentSlides;
    private int currentFactIndex = 0;
    private PersistentMusic persistentMusic;
    private int starsEarned = 0;

    private string highScoreKey => "HighScore_Level_" + SceneManager.GetActiveScene().buildIndex;

    void Start()
    {
        persistentMusic = FindObjectOfType<PersistentMusic>();
    }

    public void RegisterGarbageEaten(int points)
    {
        if (GameState.isGameOver) return;

        garbageEaten++;
        totalScore += points;

        if (garbageEaten >= targetGarbageCount)
        {
            GameState.isGameOver = true;
            ShowScorePanel();
        }
    }

    private void AnimatePanelBounce(GameObject panel, float finalScale = 1f)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        float bounceScale = finalScale * 1.2f;

        LeanTween.scale(rt, Vector3.one * bounceScale, 0.3f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.scale(rt, Vector3.one * finalScale, 0.1f)
                    .setIgnoreTimeScale(true);
            });
    }

    private void ShowScorePanel()
    {
        if (!GameState.isGameOver) return;

        int previousHighScore = PlayerPrefs.GetInt(highScoreKey, 0);

        if (totalScore > previousHighScore)
        {
            PlayerPrefs.SetInt(highScoreKey, totalScore);
            if (newRecordText != null) newRecordText.gameObject.SetActive(true);
            previousHighScore = totalScore; // ✅ Update for display immediately
        }
        else
        {
            if (newRecordText != null) newRecordText.gameObject.SetActive(false);
        }

        if (highScoreText != null)
            highScoreText.text = "High Score: " + previousHighScore;

        StartCoroutine(AnimateScore());
        garbageText.text = "Garbage Eaten: " + garbageEaten;

        scorePanel.SetActive(true);
        AnimatePanelBounce(scorePanel);
        ShowStars(totalScore);

        if (audioSource != null && gameCompleteSound != null)
            audioSource.PlayOneShot(gameCompleteSound);

        // Try existing reference first
        if (persistentMusic != null)
        {
            persistentMusic.FadeOutAndDestroyNow();
        }
        else
        {
            // Fallback: find the currently active PersistentMusic
            PersistentMusic[] musicObjects = GameObject.FindObjectsOfType<PersistentMusic>();
            foreach (var music in musicObjects)
            {
                if (music.gameObject.activeInHierarchy)
                {
                    music.FadeOutAndDestroyNow();
                    break;
                }
            }
        }
        Time.timeScale = 0f;
    }


    private IEnumerator AnimateScore()
    {
        int displayScore = 0;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            displayScore = Mathf.RoundToInt(Mathf.Lerp(0, totalScore, t));
            if (scoreText != null) scoreText.text = "Score: " + displayScore;
            yield return null;
        }

        scoreText.text = "Score: " + totalScore;
    }

    public void StartSequence(int score, int garbageCount)
    {
        if (GameState.isGameOver) return;

        GameState.isGameOver = true;
        garbageEaten = garbageCount;
        totalScore = score;
        ShowScorePanel();
    }

    public void ShowFactPanel()
    {
        scorePanel.SetActive(false);
        factPanel.SetActive(true);
        AnimatePanelBounce(factPanel);

        if (waterPollutionFacts.Length > 0)
        {
            currentFactIndex = Random.Range(0, waterPollutionFacts.Length);
            factText.text = waterPollutionFacts[currentFactIndex];
        }
    }

    public void ShowCurrentFactImages()
    {
        ShowImageCarousel(currentFactIndex);
    }

    public void ShowNextPanel()
    {
        factPanel.SetActive(false);
        nextLevelPanel.SetActive(true);

        if (nextLevelScoreText != null)
            nextLevelScoreText.text = "Score: " + totalScore;

        if (nextLevelGarbageText != null)
            nextLevelGarbageText.text = "Garbage Eaten: " + garbageEaten;

        int savedHighScore = PlayerPrefs.GetInt(highScoreKey, 0);
        bool isNewRecord = savedHighScore == totalScore;

        if (nextLevelHighScoreText != null)
            nextLevelHighScoreText.text = "High Score: " + savedHighScore;

        if (isNewRecord && nextLevelNewRecordText != null)
            nextLevelNewRecordText.gameObject.SetActive(true);
        else if (nextLevelNewRecordText != null)
            nextLevelNewRecordText.gameObject.SetActive(false);

        AnimatePanelBounce(nextLevelPanel);

        if (starsEarned >= 1 && nextStar1 != null) nextStar1.SetActive(true); else nextStar1?.SetActive(false);
        if (starsEarned >= 2 && nextStar2 != null) nextStar2.SetActive(true); else nextStar2?.SetActive(false);
        if (starsEarned >= 3 && nextStar3 != null) nextStar3.SetActive(true); else nextStar3?.SetActive(false);
    }

    private void ShowAndFadeOutPanel(CanvasGroup panelGroup, float fadeDuration = 1f, float displayTime = 2f)
    {
        Time.timeScale = 0f;
        panelGroup.gameObject.SetActive(true);
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;

        LeanTween.alphaCanvas(panelGroup, 1f, fadeDuration)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(displayTime, () =>
                {
                    LeanTween.alphaCanvas(panelGroup, 0f, fadeDuration)
                        .setIgnoreTimeScale(true)
                        .setOnComplete(() =>
                        {
                            panelGroup.gameObject.SetActive(false);
                        });
                }).setIgnoreTimeScale(true);
            });
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        GameState.isGameOver = false;
        StartCoroutine(RunSceneTransition(SceneManager.GetActiveScene().name));
    }

    public int[] playableLevelIndexes = { 2 };

    public void NextLevel()
{
    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    int currentIndexInList = System.Array.IndexOf(playableLevelIndexes, currentSceneIndex);

    if (currentIndexInList >= 0 && currentIndexInList < playableLevelIndexes.Length - 1)
    {
        int nextIndex = playableLevelIndexes[currentIndexInList + 1];

        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        Time.timeScale = 1f;
        GameState.isGameOver = false;

        StartCoroutine(RunSceneTransition(sceneName));
    }
    else
    {
        if (endGamePanelGroup != null)
        {
            ShowAndFadeOutPanel(endGamePanelGroup);
        }
    }
}


    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        GameState.isGameOver = false;
        StartCoroutine(RunSceneTransition("Main Menu"));
    }

    private IEnumerator RunSceneTransition(string sceneName)
    {
        if (endingSceneTransition != null)
        {
            endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }

        SceneManager.LoadScene(sceneName);
    }

    private void ShowStars(int score)
    {
        starsEarned = 0;

        if (score >= oneStarScore) starsEarned = 1;
        if (score >= twoStarScore) starsEarned = 2;
        if (score >= threeStarScore) starsEarned = 3;

        StartCoroutine(AnimateStarsSequentially(score));
    }

    private IEnumerator AnimateStarsSequentially(int score)
    {
        if (score >= oneStarScore && star1 != null)
        {
            star1.SetActive(true);
            AnimateStar(star1, 2f);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        if (score >= twoStarScore && star2 != null)
        {
            star2.SetActive(true);
            AnimateStar(star2, 2.7f);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        if (score >= threeStarScore && star3 != null)
        {
            star3.SetActive(true);
            AnimateStar(star3, 2f);
        }
    }

    private void AnimateStar(GameObject star, float finalScale)
    {
        RectTransform rt = star.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        float bounceScale = finalScale * 1.2f;

        LeanTween.scale(rt, Vector3.one * bounceScale, 0.3f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.scale(rt, Vector3.one * finalScale, 0.1f)
                    .setIgnoreTimeScale(true);
            });
    }

    public void ShowImageCarousel(int factIndex)
    {
        if (factSlides == null || factSlides.Length <= factIndex) return;

        currentSlides = factSlides[factIndex].slides;
        currentSlideIndex = 0;

        imageCarouselPanel.SetActive(true);

        RectTransform rt = imageCarouselPanel.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        LeanTween.scale(rt, Vector3.one, 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);

        UpdateSlideDisplay();
    }

    public void ShowNextSlide()
    {
        if (currentSlides == null || currentSlides.Length == 0) return;
        currentSlideIndex = (currentSlideIndex + 1) % currentSlides.Length;
        UpdateSlideDisplay();
    }

    public void ShowPreviousSlide()
    {
        if (currentSlides == null || currentSlides.Length == 0) return;
        currentSlideIndex = (currentSlideIndex - 1 + currentSlides.Length) % currentSlides.Length;
        UpdateSlideDisplay();
    }

    public void CloseImageCarousel()
    {
        RectTransform rt = imageCarouselPanel.GetComponent<RectTransform>();

        LeanTween.scale(rt, Vector3.zero, 0.3f)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                imageCarouselPanel.SetActive(false);
            });
    }

    private void UpdateSlideDisplay()
    {
        if (slideDisplay != null && currentSlides != null && currentSlides.Length > 0)
        {
            slideDisplay.sprite = currentSlides[currentSlideIndex];
        }
    }

    public int GetGarbageCount()
    {
        return garbageEaten;
    }
}
