using UnityEngine;

public class AboutButtonScript : MonoBehaviour
{
    public GameObject aboutPanel; // Assign your panel in Inspector
    private RectTransform panelTransform;

    private void Awake()
    {
        panelTransform = aboutPanel.GetComponent<RectTransform>();
        aboutPanel.SetActive(false); // Start hidden
        panelTransform.localScale = Vector3.zero; // Ensure hidden state
    }

    public void ToggleAboutPanel()
    {
        if (!aboutPanel.activeSelf)
        {
            // Show with bounce
            aboutPanel.SetActive(true);
            panelTransform.localScale = Vector3.zero;
            LeanTween.scale(panelTransform, Vector3.one, 0.4f).setEaseOutBack();
        }
        else
        {
            // Hide with scale-down
            LeanTween.scale(panelTransform, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() =>
            {
                aboutPanel.SetActive(false);
            });
        }
    }

    public void ClosePanel()
    {
        if (aboutPanel.activeSelf)
        {
            // Same as above: hide with animation
            LeanTween.scale(panelTransform, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() =>
            {
                aboutPanel.SetActive(false);
            });
        }
    }
}
