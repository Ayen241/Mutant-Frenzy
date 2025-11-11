using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;

public class VoiceControl : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    private Dictionary<string, System.Action> commands = new Dictionary<string, System.Action>();

    // Reference to the Shark GameObject
    public GameObject shark;

    // Cooldown for quick successive commands (in seconds)
    private float commandCooldown = 0.5f;
    private float lastCommandTime = 0f;

    void Start()
    {
        // Initialize the commands for movement
        commands.Add("up", MoveUp);
        commands.Add("down", MoveDown);
        commands.Add("left", MoveLeft);
        commands.Add("right", MoveRight);

        // Setup DictationRecognizer for continuous speech input
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationComplete += OnDictationComplete;

        // Start the DictationRecognizer
        dictationRecognizer.Start();
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        // Check if enough time has passed since the last command to prevent too fast input
        if (Time.time - lastCommandTime < commandCooldown) return;

        // Log the result and check if it matches a known command
        Debug.Log("Recognized Speech: " + text);

        string recognizedCommand = text.ToLower().Trim();

        // Execute the corresponding command if recognized
        if (commands.ContainsKey(recognizedCommand))
        {
            lastCommandTime = Time.time;
            commands[recognizedCommand].Invoke();
        }
    }

    private void OnDictationHypothesis(string text)
    {
        // Optionally handle partial speech (live feedback)
        Debug.Log("Hypothesis: " + text);
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        // Restart dictation if needed (in case it's stopped or needs reset)
        if (cause != DictationCompletionCause.Complete)
        {
            dictationRecognizer.Start();
        }
    }

    // Action to move the shark up
    private void MoveUp()
    {
        Debug.Log("Move Shark Up");
        shark.transform.position = new Vector3(shark.transform.position.x, shark.transform.position.y + Time.deltaTime * 5f, shark.transform.position.z); // Moves on the Y-axis
    }

    // Action to move the shark down
    private void MoveDown()
    {
        Debug.Log("Move Shark Down");
        shark.transform.position = new Vector3(shark.transform.position.x, shark.transform.position.y - Time.deltaTime * 5f, shark.transform.position.z); // Moves on the Y-axis
    }

    // Action to move the shark left
    private void MoveLeft()
    {
        Debug.Log("Move Shark Left");
        shark.transform.position = new Vector3(shark.transform.position.x - Time.deltaTime * 1000, shark.transform.position.y, shark.transform.position.z); // Moves on the X-axis
    }

    // Action to move the shark right
    private void MoveRight()
    {
        Debug.Log("Move Shark Right");
        shark.transform.position = new Vector3(shark.transform.position.x + Time.deltaTime * 5f, shark.transform.position.y, shark.transform.position.z); // Moves on the X-axis
    }

    void OnDisable()
    {
        // Stop the recognizer when the game object is disabled
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= OnDictationComplete;
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}
