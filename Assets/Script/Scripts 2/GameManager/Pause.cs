using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager2 : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;
    public GameObject pauseMenuUI;

    private RectTransform pauseMenuTransform;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuTransform = pauseMenuUI.GetComponent<RectTransform>();

        if (_startingSceneTransition != null)
        {
            _startingSceneTransition.SetActive(true);
            StartCoroutine(DisableStartingSceneTransitionAfterDelay(5f));
        }

        pauseMenuUI.SetActive(false);
        pauseMenuTransform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameState.isGameOver)
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        if (GameState.isGameOver) return;
        if (!isPaused)
        {
            isPaused = true;
            pauseMenuUI.SetActive(true);
            pauseMenuTransform.localScale = Vector3.zero;

            LeanTween.scale(pauseMenuTransform, Vector3.one, 0.4f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    Time.timeScale = 0f;
                });
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;

            LeanTween.scale(pauseMenuTransform, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    pauseMenuUI.SetActive(false);
                });
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        isPaused = false;

        LeanTween.scale(pauseMenuTransform, Vector3.zero, 0.3f)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                pauseMenuUI.SetActive(false);
            });
    }

    private IEnumerator DisableStartingSceneTransitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startingSceneTransition.SetActive(false);
    }

    public void QuitGame()
    {
        StartCoroutine(LoadLobbyAfterTransition());
    }

    private IEnumerator LoadLobbyAfterTransition()
    {
        if (_endingSceneTransition != null)
        {
            _endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }

        SceneManager.LoadScene("Main Menu");

        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }
}
