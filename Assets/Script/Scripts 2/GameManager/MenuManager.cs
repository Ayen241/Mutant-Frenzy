using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenu;
    public GameObject optionsMenu;

    private RectTransform mainMenuTransform;
    private RectTransform optionsMenuTransform;

    private void Awake()
    {
        mainMenuTransform = mainMenu.GetComponent<RectTransform>();
        optionsMenuTransform = optionsMenu.GetComponent<RectTransform>();

        // Ensure scale states
        mainMenuTransform.localScale = Vector3.one;
        optionsMenuTransform.localScale = Vector3.zero;

        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    // Called when "OPTIONS" is clicked
    public void ShowOptions()
    {
        LeanTween.scale(mainMenuTransform, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() =>
        {
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
            optionsMenuTransform.localScale = Vector3.zero;
            LeanTween.scale(optionsMenuTransform, Vector3.one, 0.4f).setEaseOutBack();
        });
    }

    // Called when "BACK" is clicked in options
    public void ShowMainMenu()
    {
        LeanTween.scale(optionsMenuTransform, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() =>
        {
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
            mainMenuTransform.localScale = Vector3.zero;
            LeanTween.scale(mainMenuTransform, Vector3.one, 0.4f).setEaseOutBack();
        });
    }

    // Optional: Called when "QUIT" is clicked
    public void QuitGame()
    {
        Debug.Log("Quit game");  // Works in editor
        Application.Quit();      // Works in build
    }
}
