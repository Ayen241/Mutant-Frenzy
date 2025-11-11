using UnityEngine;
using UnityEngine.UI;

public class BackgroundScalerWithCanvas : MonoBehaviour
{
    private RectTransform backgroundRectTransform;

    void Start()
    {
        backgroundRectTransform = GetComponent<RectTransform>();
        ScaleBackground();
    }

    void ScaleBackground()
    {
        if (backgroundRectTransform != null)
        {
            // Get the canvas reference and calculate the scale ratio based on screen size
            float canvasWidth = backgroundRectTransform.rect.width;
            float canvasHeight = backgroundRectTransform.rect.height;

            // Adjust the width and height of the background to fill the canvas
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Scale the background based on screen size
            float scaleX = screenWidth / canvasWidth;
            float scaleY = screenHeight / canvasHeight;

            // Apply the scale
            backgroundRectTransform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }
}
