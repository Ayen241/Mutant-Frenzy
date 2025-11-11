using UnityEngine;
using UnityEngine.UI;  // For UI components like Image

public class FunFactsManager : MonoBehaviour
{
    // Reference to the Fun Facts Panel
    public GameObject funFactsPanel;  // Assign in the Inspector
    public Image funFactsImage;  // Assign in the Inspector

    // Array of images for fun facts about water pollution
    public Sprite[] funFactImages;  // Assign the sprites in the Inspector

    void Start()
    {
        // Ensure the Fun Facts Panel is hidden at the start
        funFactsPanel.SetActive(false);
    }

    // Show the Fun Facts Panel and display a random image for the fact
    public void ShowFunFacts()
    {
        // Show the Fun Facts Panel
        funFactsPanel.SetActive(true);

        // Pick a random image for the fun fact
        int randomIndex = Random.Range(0, funFactImages.Length);  // Generate a random index
        Sprite randomFactImage = funFactImages[randomIndex];  // Select a random sprite from the array

        // Set the random image as the sprite for the Fun Facts Image component
        funFactsImage.sprite = randomFactImage;
    }
}
