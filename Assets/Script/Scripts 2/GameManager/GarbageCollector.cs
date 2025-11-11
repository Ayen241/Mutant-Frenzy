using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    public int garbageEaten = 0;
    public int targetGarbageCount = 10;

    public GameObject gameCompletePanel;
    public GameObject funFactsPanel;
    public FunFactsManager funFactsManager;

    public void NotifyGarbageEaten()
    {
        garbageEaten++;
        if (garbageEaten >= targetGarbageCount)
        {
            ShowFunFacts();
        }
    }

    void ShowFunFacts()
    {
        Debug.Log("Displaying Fun Facts Panel!");
        gameCompletePanel.SetActive(false);
        funFactsManager.ShowFunFacts();
        Time.timeScale = 0f;
    }

    public void OnFunFactsButtonClicked()
    {
        funFactsPanel.SetActive(false);
        gameCompletePanel.SetActive(true);
    }
}
