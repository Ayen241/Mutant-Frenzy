using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class toLobby : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;

    private void Start()
    {
        _startingSceneTransition.SetActive(true);
        StartCoroutine(DisableStartingSceneTransitionAfterDelay(5f));
    }

    private IEnumerator DisableStartingSceneTransitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startingSceneTransition.SetActive(false);
    }

    // ✅ Call this via Button OnClick
    public void GoToLobby()
    {
        StartCoroutine(LoadLobbyAfterTransition());
    }

    private IEnumerator LoadLobbyAfterTransition()
    {
        _endingSceneTransition.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Lobby");
    }
}
