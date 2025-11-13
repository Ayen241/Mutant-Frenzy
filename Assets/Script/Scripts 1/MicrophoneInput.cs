using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

public class VoiceControl : MonoBehaviour
{
#if UNITY_STANDALONE_WIN || UNITY_WSA
    private DictationRecognizer dictationRecognizer;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartVoiceRecognition(string gameObjectName);
    
    [DllImport("__Internal")]
    private static extern void StopVoiceRecognition();
    
    [DllImport("__Internal")]
    private static extern bool IsVoiceRecognitionSupported();
#endif

    private Dictionary<string, System.Action> commands = new Dictionary<string, System.Action>();
    private Dictionary<string, List<string>> commandSynonyms = new Dictionary<string, List<string>>();

    // Reference to the Shark GameObject
    public GameObject shark;

    // Settings
    [Header("Voice Recognition Settings")]
    [Tooltip("Minimum time between commands to prevent accidental rapid inputs")]
    public float commandCooldown = 0.3f;
    
    [Tooltip("Enable fuzzy matching for similar sounding commands")]
    public bool enableFuzzyMatching = true;
    
    [Tooltip("Show debug logs for voice commands")]
    public bool showDebugLogs = true;

    // Status tracking
    private float lastCommandTime = 0f;
    private string lastRecognizedCommand = "";
    private bool isListening = false;

    // Visual feedback (optional)
    public UnityEngine.UI.Text statusText;
    public UnityEngine.UI.Image microphoneIndicator;

    void Start()
    {
        InitializeCommands();
        StartVoiceRecognition();
    }

    private void InitializeCommands()
    {
        // Initialize the commands for movement
        commands.Add("up", MoveUp);
        commands.Add("down", MoveDown);
        commands.Add("left", MoveLeft);
        commands.Add("right", MoveRight);

        // Add synonyms for better recognition
        commandSynonyms["up"] = new List<string> { "up", "go up", "move up", "swim up", "upward", "north" };
        commandSynonyms["down"] = new List<string> { "down", "go down", "move down", "swim down", "downward", "south" };
        commandSynonyms["left"] = new List<string> { "left", "go left", "turn left", "move left", "west" };
        commandSynonyms["right"] = new List<string> { "right", "go right", "turn right", "move right", "east" };
    }

    private void StartVoiceRecognition()
    {
#if UNITY_STANDALONE_WIN || UNITY_WSA
        // Setup DictationRecognizer for Windows platforms
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationComplete += OnDictationComplete;

        // Start the DictationRecognizer
        dictationRecognizer.Start();
        isListening = true;
        LogDebug("Windows Speech Recognition started");
        UpdateStatus("Listening...", Color.green);
#elif UNITY_WEBGL && !UNITY_EDITOR
        // Start Web Speech API for WebGL
        StartVoiceRecognition(gameObject.name);
        LogDebug("WebGL Speech Recognition started. Please allow microphone access in your browser.");
        UpdateStatus("Requesting microphone access...", Color.yellow);
#else
        LogDebug("Voice recognition is only available in builds. Build for Windows or WebGL to test voice controls.");
        UpdateStatus("Voice recognition not available in Editor", Color.gray);
#endif
    }

    // === WebGL Callbacks ===
    
    // Called from JavaScript when speech is recognized (final result)
    public void OnSpeechRecognized(string text)
    {
        ProcessCommand(text, true);
    }

    // Called from JavaScript for interim results (faster feedback)
    public void OnInterimResult(string text)
    {
        ProcessCommand(text, false);
    }

    // Called from JavaScript when an alternative recognition is available
    public void OnSpeechAlternative(string text)
    {
        // Try alternative if main command didn't match
        if (lastRecognizedCommand == "")
        {
            ProcessCommand(text, true);
        }
    }

    // Called from JavaScript when voice recognition starts
    public void OnVoiceRecognitionStarted(string message)
    {
        isListening = true;
        UpdateStatus("Listening...", Color.green);
        LogDebug("Voice recognition started");
    }

    // Called from JavaScript when voice recognition ends
    public void OnVoiceRecognitionEnded(string message)
    {
        UpdateStatus("Reconnecting...", Color.yellow);
    }

    // Called from JavaScript when there's an error
    public void OnVoiceRecognitionError(string error)
    {
        LogDebug($"Voice recognition error: {error}");
        
        if (error == "not-supported")
        {
            UpdateStatus("Microphone not supported", Color.red);
        }
        else if (error == "not-allowed")
        {
            UpdateStatus("Microphone permission denied", Color.red);
        }
        else
        {
            UpdateStatus("Voice error - retrying...", Color.yellow);
        }
    }

#if UNITY_STANDALONE_WIN || UNITY_WSA
    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        ProcessCommand(text, true);
    }

    private void OnDictationHypothesis(string text)
    {
        // Optionally handle partial speech (live feedback)
        LogDebug("Hypothesis: " + text);
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        // Restart dictation if needed (in case it's stopped or needs reset)
        if (cause != DictationCompletionCause.Complete)
        {
            dictationRecognizer.Start();
        }
    }
#endif

    // Unified command processing for both platforms
    private void ProcessCommand(string text, bool isFinal = true)
    {
        // Check if enough time has passed since the last command to prevent too fast input
        if (Time.time - lastCommandTime < commandCooldown) return;

        string recognizedCommand = text.ToLower().Trim();
        
        // Try direct command match first
        if (commands.ContainsKey(recognizedCommand))
        {
            ExecuteCommand(recognizedCommand);
            return;
        }

        // Try synonym matching
        string matchedCommand = FindCommandBySynonym(recognizedCommand);
        if (matchedCommand != null)
        {
            ExecuteCommand(matchedCommand);
            return;
        }

        // Try fuzzy matching for similar sounding words
        if (enableFuzzyMatching && isFinal)
        {
            matchedCommand = FindCommandByFuzzyMatch(recognizedCommand);
            if (matchedCommand != null)
            {
                LogDebug($"Fuzzy matched '{recognizedCommand}' to '{matchedCommand}'");
                ExecuteCommand(matchedCommand);
                return;
            }
        }

        // No match found
        if (isFinal)
        {
            LogDebug($"Unrecognized command: '{recognizedCommand}'");
            lastRecognizedCommand = "";
        }
    }

    private void ExecuteCommand(string commandKey)
    {
        lastCommandTime = Time.time;
        lastRecognizedCommand = commandKey;
        commands[commandKey].Invoke();
        LogDebug($"Executed command: {commandKey}");
        UpdateMicrophoneIndicator();
    }

    private string FindCommandBySynonym(string text)
    {
        foreach (var kvp in commandSynonyms)
        {
            if (kvp.Value.Contains(text))
            {
                return kvp.Key;
            }
        }
        return null;
    }

    private string FindCommandByFuzzyMatch(string text)
    {
        // Check if the text contains any command keywords
        foreach (var command in commands.Keys)
        {
            if (text.Contains(command))
            {
                return command;
            }
        }

        // Check against all synonyms
        foreach (var kvp in commandSynonyms)
        {
            foreach (var synonym in kvp.Value)
            {
                if (text.Contains(synonym))
                {
                    return kvp.Key;
                }
            }
        }

        // Simple Levenshtein distance for close matches (optional)
        int bestDistance = int.MaxValue;
        string bestMatch = null;
        
        foreach (var command in commands.Keys)
        {
            int distance = LevenshteinDistance(text, command);
            if (distance <= 2 && distance < bestDistance) // Allow 2 character differences
            {
                bestDistance = distance;
                bestMatch = command;
            }
        }

        return bestMatch;
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Mathf.Min(
                    Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    private void LogDebug(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Voice Control] {message}");
        }
    }

    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
    }

    private void UpdateMicrophoneIndicator()
    {
        if (microphoneIndicator != null)
        {
            // Flash the indicator
            StartCoroutine(FlashIndicator());
        }
    }

    private System.Collections.IEnumerator FlashIndicator()
    {
        if (microphoneIndicator != null)
        {
            microphoneIndicator.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            microphoneIndicator.color = Color.white;
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
#if UNITY_STANDALONE_WIN || UNITY_WSA
        // Stop the recognizer when the game object is disabled
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= OnDictationComplete;
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
#elif UNITY_WEBGL && !UNITY_EDITOR
        StopVoiceRecognition();
#endif
    }
}
