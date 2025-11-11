using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Scene to Load")]
    public string levelSceneName;

    [Header("Transition")]
    [SerializeField] private GameObject _endingSceneTransition;

    private void OnMouseDown()
    {
        Debug.Log("Clicked on: " + gameObject.name);
        StartCoroutine(LoadSceneWithTransition());
    }

    private IEnumerator LoadSceneWithTransition()
    {
        if (_endingSceneTransition != null)
        {
            _endingSceneTransition.SetActive(true);
            yield return new WaitForSeconds(1.5f); // Adjust based on animation length
        }

        SceneManager.LoadScene(levelSceneName);
    }
}
