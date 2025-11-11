using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton;

    private void Start()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }
        else
        {
            Debug.LogWarning("Next Level Button not assigned in Inspector.");
        }
    }

    private void LoadNextLevel()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Load the next level
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels. End of game!");
            // You could load a main menu or credits scene here
        }
    }
}
