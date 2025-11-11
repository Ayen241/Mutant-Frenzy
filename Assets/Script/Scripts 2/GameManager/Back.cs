using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonWithTransition : MonoBehaviour
{
    [Header("Scene to Load")]
    [SerializeField] private string sceneToLoad;

    [Header("Transition Panels")]
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;

    private void Start()
    {
        if (_startingSceneTransition != null)
        {
            _startingSceneTransition.SetActive(true);
            StartCoroutine(DisableStartingSceneTransitionAfterDelay(5f));
        }
    }

    private IEnumerator DisableStartingSceneTransitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startingSceneTransition.SetActive(false);
    }

    public void GoToAssignedScene()
    {
        StartCoroutine(TransitionAndLoadScene());
    }

    private IEnumerator TransitionAndLoadScene()
    {
        if (_endingSceneTransition != null)
        {
            _endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name assigned to BackButtonWithTransition.");
        }
    }
}
