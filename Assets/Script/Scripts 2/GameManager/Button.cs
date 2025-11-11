using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonHandler : MonoBehaviour
{
    [Header("Transition Panels")]
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;

    private void Start()
    {
        // Play starting scene transition if assigned
        if (_startingSceneTransition != null)
        {
            _startingSceneTransition.SetActive(true);
            StartCoroutine(DisableStartingTransitionAfterDelay(5f));
        }
    }

    private IEnumerator DisableStartingTransitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startingSceneTransition.SetActive(false);
    }

    // ? Called when Play button is clicked
    public void LoadSelectionScene()
    {
        // Record current scene for back button functionality
        StartCoroutine(TransitionAndLoadScene("Selection"));
    }

    // ? Optional Quit method with transition
    public void QuitGame()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }

    private IEnumerator TransitionAndLoadScene(string sceneName)
    {
        if (_endingSceneTransition != null)
        {
            _endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }

        SceneManager.LoadScene(sceneName);
    }
}
