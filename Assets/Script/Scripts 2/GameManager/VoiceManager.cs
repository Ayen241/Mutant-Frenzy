using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif
using System.Runtime.InteropServices;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;

#if UNITY_STANDALONE_WIN || UNITY_WSA
    private KeywordRecognizer recognizer;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartVoiceRecognition(string gameObjectName);
    
    [DllImport("__Internal")]
    private static extern void StopVoiceRecognition();
#endif

    private Dictionary<string, Action> keywordMap = new();

    public Vector2 VoiceDirection { get; private set; } = Vector2.zero;
    public float voiceCommandDuration = 0.5f;
    public bool IsDashRequested { get; private set; } = false;

    private Coroutine resetCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeKeywords();
#if UNITY_STANDALONE_WIN || UNITY_WSA
        recognizer = new KeywordRecognizer(keywordMap.Keys.ToArray(), ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnKeywordsRecognized;
        recognizer.Start();
        Debug.Log("Windows Speech Recognition started");
#elif UNITY_WEBGL && !UNITY_EDITOR
        StartVoiceRecognition(gameObject.name);
        Debug.Log("WebGL Speech Recognition started. Please allow microphone access in your browser.");
#else
        Debug.LogWarning("Voice recognition is only available in builds. Build for Windows or WebGL to test voice controls.");
#endif
    }

    void InitializeKeywords()
    {
        AddKeywords(new[] { "up", "go up", "move up", "swim up" }, Vector2.up);
        AddKeywords(new[] { "down", "go down", "move down", "swim down" }, Vector2.down);
        AddKeywords(new[] { "left", "go left", "turn left", "move left" }, Vector2.left);
        AddKeywords(new[] { "right", "go right", "turn right", "move right" }, Vector2.right);

        // Dash triggers using repeated direction commands
        keywordMap["left left"] = () => TriggerDash(Vector2.left);
        keywordMap["right right"] = () => TriggerDash(Vector2.right);
        keywordMap["up up"] = () => TriggerDash(Vector2.up);
        keywordMap["down down"] = () => TriggerDash(Vector2.down);
    }

    void AddKeywords(string[] phrases, Vector2 direction)
    {
        foreach (var phrase in phrases)
        {
            if (!keywordMap.ContainsKey(phrase))
            {
                keywordMap.Add(phrase, () => SetVoiceDirection(direction));
            }
        }
    }

    // Called from JavaScript (WebGL) when speech is recognized
    public void OnSpeechRecognized(string text)
    {
        Debug.Log("Voice command: " + text);
        
        text = text.ToLower().Trim();
        
        if (keywordMap.TryGetValue(text, out Action action))
        {
            action.Invoke();
        }
        
        FindObjectOfType<ControlIndicatorUI>()?.TriggerMicActivity();
    }

#if UNITY_STANDALONE_WIN || UNITY_WSA
    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Voice command: " + args.text);
        if (keywordMap.TryGetValue(args.text, out Action action))
        {
            action.Invoke();
        }
        FindObjectOfType<ControlIndicatorUI>()?.TriggerMicActivity();

    }
#endif

    private void SetVoiceDirection(Vector2 dir)
    {
        VoiceDirection = dir;

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetDirectionAfterDelay());
    }

    private void TriggerDash(Vector2 dir)
    {
        VoiceDirection = dir;
        IsDashRequested = true;

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetDirectionAfterDelay());
        StartCoroutine(ResetDashFlag());
    }

    private IEnumerator ResetDirectionAfterDelay()
    {
        yield return new WaitForSeconds(voiceCommandDuration);
        VoiceDirection = Vector2.zero;
    }

    private IEnumerator ResetDashFlag()
    {
        yield return new WaitForSeconds(0.2f);
        IsDashRequested = false;
    }

    private void OnDestroy()
    {
#if UNITY_STANDALONE_WIN || UNITY_WSA
        if (recognizer != null)
        {
            recognizer.OnPhraseRecognized -= OnKeywordsRecognized;
            recognizer.Stop();
            recognizer.Dispose();
        }
#elif UNITY_WEBGL && !UNITY_EDITOR
        StopVoiceRecognition();
#endif
    }
}
