using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Start()
    {
        // Optional: if you want a starting scene transition at the start of the game
        _startingSceneTransition.SetActive(true);
        StartCoroutine(DisableStartingSceneTransitionAfterDelay(5f)); // Disable after 5 seconds (or adjust)
    }

    void Update()
    {
        // Press ESC to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuUI.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unpauses the game
        pauseMenuUI.SetActive(false); // Hides the pause menu
    }

    private IEnumerator DisableStartingSceneTransitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startingSceneTransition.SetActive(false);
    }

    // Quit and trigger transition to Lobby scene
    public void QuitGame()
    {
        StartCoroutine(LoadLobbyAfterTransition());

    }

    private IEnumerator LoadLobbyAfterTransition()
    {
        // Activate the ending transition effect (e.g., fade out)
        _endingSceneTransition.SetActive(true);

        // Wait for the transition to complete before loading the scene
        yield return new WaitForSeconds(1.5f); // Adjust this time based on how long you want the transition to last

        // Now load the Lobby scene
        SceneManager.LoadScene("Lobby");

        // Optionally, unpause the game (in case the scene transition was triggered during a paused state)
        isPaused = false;
        Time.timeScale = 1f; // Unpauses the game
        pauseMenuUI.SetActive(false);
    }
}
