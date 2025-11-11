using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoader : MonoBehaviour
{
    public float delay = 2.5f; // seconds to wait
    public string nextScene = "Main Menu"; // or "Level1"

    void Start()
    {
        Invoke("LoadNext", delay);
    }

    void LoadNext()
    {
        SceneManager.LoadScene(nextScene);
    }
}
