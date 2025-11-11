using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SharkHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stun Settings")]
    public float stunDuration = 1.5f;
    private bool isStunned = false;

    [Header("UI")]
    public Slider healthBar;
    public GameObject damagePopupPrefab;
    public RectTransform damagePopupParent;

    [Header("Death Panel Auto Timer")]
    public GameObject deathPanel;
    public TMP_Text countdownText;
    public float autoReturnTime = 10f;

    [Header("Effects")]
    public GameObject endingSceneTransition;

    private Animator animator;
    private Coroutine healthAnimCoroutine;
    private Coroutine countdownCoroutine;
    private bool hasInteracted = false;
    private PersistentMusic persistentMusic;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateHealthBar();

        persistentMusic = FindObjectOfType<PersistentMusic>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb"))
        {
            // Visual + audio effect
            FindObjectOfType<BombEffect>()?.TriggerExplosion(other.transform.position);

            // Damage logic
            TakeDamage(25f);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damageAmount)
{
    if (GameState.isGameOver) return;
    if (!isStunned) StartCoroutine(StunShark());

    currentHealth -= damageAmount;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    UpdateHealthBar();
    ShowDamagePopup((int)damageAmount);
    FindObjectOfType<CameraShaker>()?.Shake();

    // 💥 Reset combo system when hit
    var sharkStats = GetComponent<SharkStats>();
    if (sharkStats != null)
    {
        sharkStats.ResetCombo();
    }
    FindObjectOfType<ScoreManager>()?.ShowComboText(""); // hide visual

    if (currentHealth <= 0)
    {
        GameState.isGameOver = true;
        Die();
    }
}


    private IEnumerator StunShark()
    {
        isStunned = true;

        var movement = GetComponent<SharkMovement>();
        if (movement != null) movement.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        if (!GameState.isGameOver && movement != null)
            movement.enabled = true;

        isStunned = false;
    }

    public void ShowDamagePopup(int amount)
    {
        if (damagePopupPrefab != null && damagePopupParent != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, damagePopupParent);
            popup.GetComponent<DamagePopup>().SetDamage(amount);

            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            popup.GetComponent<RectTransform>().position = screenPos;
        }
    }


    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float targetValue = currentHealth / maxHealth;

        if (healthAnimCoroutine != null)
            StopCoroutine(healthAnimCoroutine);

        healthAnimCoroutine = StartCoroutine(AnimateHealthBar(targetValue));
    }

    private IEnumerator AnimateHealthBar(float targetValue)
    {
        float duration = 0.3f;
        float time = 0f;
        float startValue = healthBar.value;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            healthBar.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        healthBar.value = targetValue;
    }

    private void Die()
    {
        Debug.Log("Shark has died.");
        GetComponent<SharkSFX>()?.PlayDeathSound();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        var movement = GetComponent<SharkMovement>();
        if (movement != null) movement.enabled = false;

        var stats = GetComponent<SharkStats>();
        if (stats != null) stats.enabled = false;

        // 🔊 Fade out music immediately on death
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

        StartCoroutine(DelayedGameOver(4f));
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

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            AnimatePanelBounce(deathPanel); // 🔄 Animate appearance
        }

        Time.timeScale = 0f;

        countdownCoroutine = StartCoroutine(StartDeathCountdown());

        var flow = FindObjectOfType<GameCompletionFlow>();
        var stats = GetComponent<SharkStats>();

        if (flow != null && stats != null)
            flow.StartSequence(stats.GetScore(), flow.GetGarbageCount());
    }

    private IEnumerator StartDeathCountdown()
    {
        float timer = autoReturnTime;

        while (timer > 0 && !hasInteracted)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timer).ToString() + "...";

            yield return new WaitForSecondsRealtime(1f);
            timer -= 1f;
        }

        if (!hasInteracted)
            ReturnToMainMenu();
    }

    public void RestartLevel()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        GameState.isGameOver = false;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        StartCoroutine(RestartWithTransition());
    }

    private IEnumerator RestartWithTransition()
    {
        Time.timeScale = 1f;

        if (endingSceneTransition != null)
        {
            endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        GameState.isGameOver = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
