using UnityEngine;

public class PauseManager1 : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject Pause;
    public GameObject optionsMenu;

    // Called when "OPTIONS" is clicked
    public void ShowOptions()
    {
        Pause.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Called when "BACK" is clicked in options
    public void ShowPause()
    {
        optionsMenu.SetActive(false);
        Pause.SetActive(true);
    }

    // Optional: Called when "QUIT" is clicked
}
