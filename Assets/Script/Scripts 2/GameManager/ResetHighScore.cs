using UnityEngine;

public class ResetHighScores : MonoBehaviour
{
    void Awake()
    {
        for (int i = 2; i <= 4; i++) // Build indexes for Level 1 to Level 3
        {
            string key = "HighScore_Level_" + i;
            PlayerPrefs.DeleteKey(key);
            Debug.Log("Deleted: " + key);
        }

        PlayerPrefs.Save();
    }
}
